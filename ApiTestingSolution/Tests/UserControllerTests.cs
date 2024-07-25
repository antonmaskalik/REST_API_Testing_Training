using ApiTestingSolution.Enums;
using ApiTestingSolution.Helpers;
using ApiTestingSolution.Models;
using ApiTestingSolution.Services;
using NUnit.Framework.Legacy;
using RestSharp;
using System.Net;

namespace ApiTestingSolution.Tests
{
    public class UserControllerTests
    {
        [Test]
        public void AddCorrectUserTest()
        {
            var zipCodes = ZipCodeControllerService.GetAvailableZipCodes();
            var codes = JsonHelper.DeserializeJsonContent<List<string>>(zipCodes);
            var user = new User()
            {
                Age = RandomHelper.GetRandomInt(1, 60),
                Name = RandomHelper.GetRandomString(),
                Sex = Sex.MALE,
                ZipCode = codes.FirstOrDefault()
            };

            var response = UserControllerService.CreateUser(user);
            var users = UserControllerService.GetAllUsers().Users;
            zipCodes = ZipCodeControllerService.GetAvailableZipCodes();
            codes = JsonHelper.DeserializeJsonContent<List<string>>(zipCodes);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created), "Status code should be 201 Created");
                Assert.That(users, Contains.Item(user), "The application doesn't contain added user");
                Assert.That(codes, Does.Not.Contain(user.ZipCode), "The application contains used zip code");
            });
        }

        [Test]
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

        [Test]
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

        [Test]
        public void GetAllUsersTest()
        {
            (var users, var statusCode) = UserControllerService.GetAllUsers();

            Assert.Multiple(() =>
            {
                Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK), "Status code should be 200 OK");
                CollectionAssert.IsNotEmpty(users, "The application should store some users");
            });
        }

        [Test]
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

        [Test]
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

        [Test]
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

        [TestCase(Method.Put)]
        [TestCase(Method.Patch)]
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

        [TestCase(Method.Put)]
        [TestCase(Method.Patch)]
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

        [TestCase(Method.Put)]
        [TestCase(Method.Patch)]
        public void UpdateUserByIncorrectRequestBodyTest(Method method)
        {
            var userToUpdate = UserControllerService.GetAllUsers().Users.FirstOrDefault();
            var updateUserResponse = UserControllerService.UpdateUser(userToUpdate, null, method);
            var allUsers = UserControllerService.GetAllUsers().Users;

            Assert.Multiple(() =>
            {
                Assert.That(updateUserResponse.StatusCode, Is.EqualTo(HttpStatusCode.Conflict), "Status code should be 409 Conflict");
                Assert.That(allUsers, Does.Contain(userToUpdate), $"The application should contain the old user: {userToUpdate}");
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
        */
    }
}
