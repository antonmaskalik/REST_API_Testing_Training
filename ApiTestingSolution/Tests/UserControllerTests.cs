using ApiTestingSolution.Enums;
using ApiTestingSolution.Helpers;
using ApiTestingSolution.Models;
using ApiTestingSolution.Services;
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
            var users = UserControllerService.GetAllUsers();
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
            var user = UserControllerService.GetAllUsers().FirstOrDefault();
            user.Name += RandomHelper.GetRandomString();
            user.Age += RandomHelper.GetRandomInt(10);

            var response = UserControllerService.CreateUser(user);
            var users = UserControllerService.GetAllUsers();

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.FailedDependency), "Status code should be 424 Failed Dependency");
                Assert.That(users, Does.Not.Contain(user), "The application should not contain the user");
            });
        }

        [Test]
        public void AddUserWithIncorrectNameAndSexTest()
        {
            var user = UserControllerService.GetAllUsers().First();
            var zipCodes = ZipCodeControllerService.GetAvailableZipCodes();
            user.ZipCode = JsonHelper.DeserializeJsonContent<List<string>>(zipCodes).First();
            user.Age = RandomHelper.GetRandomInt(10);

            var response = UserControllerService.CreateUser(user);
            var users = UserControllerService.GetAllUsers();

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest), "Status code should be 400 Bad Request");
                Assert.That(users, Does.Not.Contain(user), "The application should not contain the user");
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
        */
    }
}
