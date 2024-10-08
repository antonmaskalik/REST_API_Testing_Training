﻿using Allure.NUnit;
using Allure.NUnit.Attributes;
using ApiTestingSolution.Enums;
using ApiTestingSolution.Helpers;
using ApiTestingSolution.Models;
using ApiTestingSolution.Services;
using NUnit.Framework.Legacy;
using RestSharp;
using System.Net;

namespace ApiTestingSolution.Tests
{
    [AllureNUnit]
    [AllureSuite("Tests - User controller")]
    public class UserControllerTests
    {
        [Test, Order(2)]
        [AllureFeature("AddCorrectUserTest")]
        [AllureStory("Validate add user with correct data")]
        public void AddCorrectUserTest()
        {
            var user = UserHelpers.GetRandomUsersList().FirstOrDefault();

            var response = UserControllerService.CreateUser(user);
            var users = UserControllerService.GetAllUsers().Users;
            var zipCodes = ZipCodeControllerService.GetAvailableZipCodes();
            var codes = JsonHelper.DeserializeJsonContent<List<string>>(zipCodes);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created), "Status code should be 201 Created");
                Assert.That(users, Contains.Item(user), "The application doesn't contain added user");
                Assert.That(codes, Does.Not.Contain(user.ZipCode), "The application contains used zip code");
            });
        }

        [Test, Order(3)]
        [AllureFeature("AddUserWithIncorrectZipCodeTest")]
        [AllureStory("Validate add user with incorrect zip code")]
        public void AddUserWithIncorrectZipCodeTest()
        {
            var user = UserControllerService.GetAllUsers().Users.FirstOrDefault();
            user.Name += RandomHelper.GetRandomString();
            user.Age += RandomHelper.GetRandomInt(10);

            var response = UserControllerService.CreateUser(user);
            var users = UserControllerService.GetAllUsers().Users;

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.FailedDependency), "Status code should be 424 Failed Dependency");
                Assert.That(users, Does.Not.Contain(user), "The application should not contain the user");
            });
        }

        [Test, Order(3)]
        [AllureFeature("AddUserWithIncorrectNameAndSexTest")]
        [AllureStory("Validate add user with incorrect Name and Sex")]
        public void AddUserWithIncorrectNameAndSexTest()
        {
            var user = UserControllerService.GetAllUsers().Users.First();
            var zipCodes = ZipCodeControllerService.GetAvailableZipCodes();
            user.ZipCode = JsonHelper.DeserializeJsonContent<List<string>>(zipCodes).First();
            user.Age = RandomHelper.GetRandomInt(10);

            var response = UserControllerService.CreateUser(user);
            var users = UserControllerService.GetAllUsers().Users;

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest), "Status code should be 400 Bad Request");
                Assert.That(users, Does.Not.Contain(user), "The application should not contain the user");
            });
        }

        [Test, Order(4)]
        [AllureFeature("GetAllUsersTest")]
        [AllureStory("Validate getting all existing users from the app")]
        public void GetAllUsersTest()
        {
            (var users, var statusCode) = UserControllerService.GetAllUsers();

            Assert.Multiple(() =>
            {
                Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Status code should be 200 OK");
                CollectionAssert.IsNotEmpty(users, "The application should store some users");
            });
        }

        [Test, Order(4)]
        [AllureFeature("GetAllUsersByOlderThanParameterTest")]
        [AllureStory("Validate getting users by parameter Older than")]
        public void GetAllUsersByOlderThanParameterTest()
        {
            var olderThanValue = 20;

            (var users, var statusCode) = UserControllerService.GetAllUsers(olderThan: olderThanValue);

            Assert.Multiple(() =>
            {
                Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Status code should be 200 OK");
                Assert.That(users.All(user => user.Age > olderThanValue), Is.True, $"The list of users should only contain users older than {olderThanValue}");
            });
        }

        [Test, Order(4)]
        [AllureFeature("GetAllUsersByYoungerThanParameterTest")]
        [AllureStory("Validate getting users by parameter Younger than")]
        public void GetAllUsersByYoungerThanParameterTest()
        {
            var youngerThanValue = 20;

            (var users, var statusCode) = UserControllerService.GetAllUsers(youngerThan: youngerThanValue);

            Assert.Multiple(() =>
            {
                Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Status code should be 200 OK");
                Assert.That(users.All(user => user.Age < youngerThanValue), Is.True, $"The list of users should only contain users younger than {youngerThanValue}");
            });
        }

        [Test, Order(4)]
        [AllureFeature("GetAllUsersBySexParameterTest")]
        [AllureStory("Validate getting users by parameter Sex equal to")]
        public void GetAllUsersBySexParameterTest()
        {
            var sex = Sex.FEMALE;

            (var users, var statusCode) = UserControllerService.GetAllUsers(sex: sex);

            Assert.Multiple(() =>
            {
                Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Status code should be 200 OK");
                Assert.That(users.All(user => user.Sex == sex), Is.True,
                    $"The list of users should only contain users with the sex of {sex}");
            });
        }

        [TestCase(Method.Put), Order(5)]
        [TestCase(Method.Patch)]
        [AllureFeature("UpdateUserTest")]
        [AllureStory("Validate updating user by correct data")]
        public void UpdateUserTest(Method method)
        {
            var zipCodes = ZipCodeControllerService.GetAvailableZipCodes();
            var codes = JsonHelper.DeserializeJsonContent<List<string>>(zipCodes);
            var userToUpdate = UserControllerService.GetAllUsers().Users.FirstOrDefault();
            var userNewValues = (User)userToUpdate.Clone();
            userNewValues.Age = RandomHelper.GetRandomInt(1, 60);
            userNewValues.Name = RandomHelper.GetRandomString();
            userNewValues.Sex = Sex.FEMALE;
            userNewValues.ZipCode = codes.FirstOrDefault();

            var updateUserResponse = UserControllerService.UpdateUser(userToUpdate, userNewValues, method);
            var allUsers = UserControllerService.GetAllUsers().Users;

            Assert.Multiple(() =>
            {
                Assert.That(updateUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Status code should be 200 OK");
                Assert.That(allUsers, Does.Not.Contain(userToUpdate), $"The application should not contain the old user: {userToUpdate}");
                Assert.That(allUsers, Does.Contain(userNewValues), $"The application should contain the updated user: {userNewValues}");
            });
        }

        [TestCase(Method.Put), Order(5)]
        [TestCase(Method.Patch)]
        [AllureFeature("UpdateUserByIncorrectZipCodeTest")]
        [AllureStory("Validate updating user by incorrect zip code")]
        public void UpdateUserByIncorrectZipCodeTest(Method method)
        {
            var userToUpdate = UserControllerService.GetAllUsers().Users.FirstOrDefault();
            var userNewValues = (User)userToUpdate.Clone();
            userNewValues.Age = RandomHelper.GetRandomInt(1, 60);
            userNewValues.Name = RandomHelper.GetRandomString();
            userNewValues.Sex = Sex.FEMALE;
            userNewValues.ZipCode = RandomHelper.GetRandomString();

            var updateUserResponse = UserControllerService.UpdateUser(userToUpdate, userNewValues, method);
            var allUsers = UserControllerService.GetAllUsers().Users;

            Assert.Multiple(() =>
            {
                Assert.That(updateUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.FailedDependency), "Status code should be 424 Failed Dependency");
                Assert.That(allUsers, Does.Contain(userToUpdate), $"The application should contain the old user: {userToUpdate}");
                Assert.That(allUsers, Does.Not.Contain(userNewValues), $"The application should not contain the updated user: {userNewValues}");
            });
        }

        [TestCase(Method.Put), Order(5)]
        [TestCase(Method.Patch)]
        [AllureFeature("UpdateUserByIncorrectRequestBodyTest")]
        [AllureStory("Validate updating user by incorrect body request")]
        public void UpdateUserByIncorrectRequestBodyTest(Method method)
        {
            var userToUpdate = UserControllerService.GetAllUsers().Users.FirstOrDefault();
            var updateUserResponse = UserControllerService.UpdateUser(userToUpdate, null, method);
            var allUsers = UserControllerService.GetAllUsers().Users;

            Assert.Multiple(() =>
            {
                Assert.That(updateUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest), "Status code should be 400 BadRequest");
                Assert.That(allUsers, Does.Contain(userToUpdate), $"The application should contain the old user: {userToUpdate}");
            });
        }

        [Test, Order(10)]
        [AllureFeature("DeleteUser")]
        [AllureStory("Validate delete user")]
        public void DeleteUser()
        {
            var userToDelete = UserControllerService.GetAllUsers().Users.FirstOrDefault();

            var response = UserControllerService.DeleteUser(userToDelete);
            var zipCodes = ZipCodeControllerService.GetAvailableZipCodes();
            var codes = JsonHelper.DeserializeJsonContent<List<string>>(zipCodes);
            var users = UserControllerService.GetAllUsers().Users;

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent), "Status code should be 204 No Content");
                Assert.That(users, Does.Not.Contain(userToDelete), $"{userToDelete} is not deleated");
                Assert.That(codes, Does.Contain(userToDelete.ZipCode), $"Zip code: {userToDelete.ZipCode} should be avaailible");
            });
        }

        [Test, Order(8)]
        [AllureFeature("DeleteByRequiredFieldsOnlyTest")]
        [AllureStory("Validate delete user using required fields only")]
        public void DeleteByRequiredFieldsOnlyTest()
        {
            var userToDelete = UserControllerService.GetAllUsers().Users.FirstOrDefault();
            var jsonBody = $"{{ \"name\": \"{userToDelete.Name}\", \"sex\": \"{userToDelete.Sex}\" }}";

            var response = UserControllerService.DeleteUser(jsonBody);
            var zipCodes = ZipCodeControllerService.GetAvailableZipCodes();
            var codes = JsonHelper.DeserializeJsonContent<List<string>>(zipCodes);
            var users = UserControllerService.GetAllUsers().Users;

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent), "Status code should be 204 No Content");
                Assert.That(users, Does.Not.Contain(userToDelete), $"{userToDelete} is not deleated");
                Assert.That(codes, Does.Contain(userToDelete.ZipCode), $"Zip code: {userToDelete.ZipCode} should be avaailible");
            });
        }

        [Test, Order(7)]
        [AllureFeature("DeleteByMissedRequiredFieldTest")]
        [AllureStory("Validate delete user without using required fields")]
        public void DeleteByMissedRequiredFieldTest()
        {
            var userToDelete = UserControllerService.GetAllUsers().Users.FirstOrDefault();
            var jsonBody = $"{{ \"age\": \"{userToDelete.Age}\", \"sex\": \"{userToDelete.Sex}\", \"zipCode\": \"{userToDelete.ZipCode}\" }}";
            
            var response = UserControllerService.DeleteUser(jsonBody);
            var zipCodes = ZipCodeControllerService.GetAvailableZipCodes();
            var codes = JsonHelper.DeserializeJsonContent<List<string>>(zipCodes);
            var users = UserControllerService.GetAllUsers().Users;

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict), "Status code should be 409 Conflict");
                Assert.That(users, Does.Contain(userToDelete), $"{userToDelete} is deleated");
            });
        }

        [Test, Order(9)]
        [AllureFeature("UploadFileWithCorrectUsersDataTest")]
        [AllureStory("Validate upload file with correct users data")]
        public void UploadFileWithCorrectUsersDataTest()
        {
            var file = UserHelpers.CreateJsonFileWithCorrectUsers();
            var usersFromFile = file.Users;
            var usedCodes = usersFromFile.Select(user => user.ZipCode).ToList();

            var response = UserControllerService.UploadFileWithUsers(file.FilePath);
            var users = UserControllerService.GetAllUsers().Users;
            var zipCodes = ZipCodeControllerService.GetAvailableZipCodes();
            var codes = JsonHelper.DeserializeJsonContent<List<string>>(zipCodes);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created), "Status code should be 201 Created");
                CollectionAssert.AreEqual(usersFromFile, users, "Users are not uploaded");
                CollectionAssert.IsNotSubsetOf(usedCodes, codes, "Zip codes are not deleted from the availible Zip codes list");
            });
        }

        [Test, Order(6)]
        [AllureFeature("UploadFileWithIncorrectZipCodeForUserTest")]
        [AllureStory("Validate upload file with incorrect zip code for some user")]
        public void UploadFileWithIncorrectZipCodeForUserTest()
        {
            var file = UserHelpers.CreateJsonFileWithIncorrectZipCodeForUser();
            var usersFromFile = file.Users;
            var usedCodes = usersFromFile.Select(user => user.ZipCode);
            var filePath = file.FilePath;

            var response = UserControllerService.UploadFileWithUsers(filePath);
            var users = UserControllerService.GetAllUsers().Users;
            var zipCodes = ZipCodeControllerService.GetAvailableZipCodes();
            var codes = JsonHelper.DeserializeJsonContent<List<string>>(zipCodes);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.FailedDependency), "Status code should be 404 Failed Dependency");
                CollectionAssert.IsNotSubsetOf(usersFromFile, users, "Users are uploaded");
            });
        }

        [Test, Order(6)]
        [AllureFeature("UploadFileWithMissedRequiredFieldTest")]
        [AllureStory("Validate upload file with missed required fields for some user")]
        public void UploadFileWithMissedRequiredFieldTest()
        {
            var file = UserHelpers.CreateJsonFileWithMissedRequiredField();
            var usersFromFile = file.Users;
            var usedCodes = usersFromFile.Select(user => user.ZipCode).ToList();
            var filePath = file.FilePath;

            var response = UserControllerService.UploadFileWithUsers(filePath);
            var users = UserControllerService.GetAllUsers().Users;
            var zipCodes = ZipCodeControllerService.GetAvailableZipCodes();
            var codes = JsonHelper.DeserializeJsonContent<List<string>>(zipCodes);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict), "Status code should be 409 Conflict");
                CollectionAssert.IsNotSubsetOf(usersFromFile, users, "Users are uploaded");
            });
        }

        /*
        Bugs found:
        1. Incorrect response code the same user is added. 
        Steps to reproduce:
           - Given I am authorized user 
           - Send POST request to /users endpoint and Request body contains user 
             to add with the same name and sex as existing user in the system

        Expected result: Get response code "400 Bad Request" and the user IS NOT added to app
        Actual result: Get response code "201 Created" and the user IS added to app

        2. The first user is removed from the app. 
        Steps to reproduce:
           - Given I am authorized user 
           - Send  PUT/PATCH request to /users endpoint with Request body that contains 
             user to update with new values and new zip code is incorrect (unavailable) 

        Expected result: Get response code "424 Failed Dependency" and the user IS NOT updated
        Actual result: The first user is removed from the app and get response code "424 Failed Dependency" 

        3. Incorrect response code when updating user with invalid body. 
        Steps to reproduce:
           - Given I am authorized user 
           - Send  PUT/PATCH request to /users endpoint with Request body that contains 
             user to update with new values required fields are missed  

        Expected result: Get response code "409 Conflict" and the user is not updated
        Actual result: Get response code "400 Bad Request" and the user is not updated" 

        4. User is not deleted with body containing only required fields. 
        Steps to reproduce:
           - Given I am authorized user 
           - Send  DELETE request to /users endpoint with Request body containing only required fields  

        Expected result: Get response code "204 No Content", the user is deleted and 
                         Zip code is returned in list of available zip codes 
        Actual result: Get response code "204 No Content", the user is NOT deleted and 
                       Zip code is NOT returned in list of available zip codes  

        5. Existing users are deleted from app by request to /users/upload endpoint. 
        Steps to reproduce:
           - Given I am authorized user 
           - Send POST request to /users/upload endpoint and Request body contains
             json file with array of users to upload. 

        Expected result: Get response code "201 Created" and All users are replaced with users from file  
        Actual result: Get response code "500 InternalServerError" and All users are removed from the app
        */
    }
}
