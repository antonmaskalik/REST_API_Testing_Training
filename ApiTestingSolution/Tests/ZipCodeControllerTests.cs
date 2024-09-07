using ApiTestingSolution.Services;
using System.Net;
using ApiTestingSolution.Helpers;
using NUnit.Framework.Legacy;
using Allure.NUnit.Attributes;
using Allure.NUnit;

namespace ApiTestingSolution.Tests
{
    [AllureNUnit]
    [AllureSuite("Tests - ZipCode controller")]
    public class ZipCodeControllerTests
    {
        [Test, Order(1)]
        [AllureFeature("GetAvailibleZipCodesTest")]
        [AllureStory("Validate get all availible zip codes from the app")]

        public void GetAvailibleZipCodesTest()
        {
            var response = ZipCodeControllerService.GetAvailableZipCodes();
            var codes = JsonHelper.DeserializeJsonContent<List<string>>(response);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Status code should be 200 OK");
                CollectionAssert.IsNotEmpty(codes, "Contant should contain some ZIP codes");
            });
        }

        [Test, Order(2)]
        [AllureFeature("ExpandAvailibleZipCodesTest")]
        [AllureStory("Validate adding correct zip code to the app")]
        public void ExpandAvailibleZipCodesTest()
        {
            var zipCodes = new List<string>()
            {
                RandomHelper.GetRandomString(5),
                RandomHelper.GetRandomString(8)
            };

            var response = ZipCodeControllerService.ExpendAvailableZipCodes(zipCodes);
            var actualCodes = JsonHelper.DeserializeJsonContent<List<string>>(response);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created), "Status code should be 201 Created");
                CollectionAssert.IsSubsetOf(zipCodes, actualCodes, "The application doesn't contain added ZIP codes");
            });
        }

        [Test]
        [AllureFeature("ExpandAvailibleZipCodesTest")]
        [AllureStory("Validate adding incorrect zip code to the app")]
        public void ExpandAvailibleZipCodesByDuplicationsTest()
        {
            var response = ZipCodeControllerService.GetAvailableZipCodes();
            var actualCodes = JsonHelper.DeserializeJsonContent<List<string>>(response);
            var zipCodes = actualCodes;
            zipCodes.Add(actualCodes.First());

            response = ZipCodeControllerService.ExpendAvailableZipCodes(zipCodes);
            actualCodes = JsonHelper.DeserializeJsonContent<List<string>>(response);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created), "Status code should be 201 Created");
                Assert.That(actualCodes, Is.Unique, "The list of the app ZIP codes contains dublications");
            });
        }

        /*
        Bugs found:
        1. Incorrect response code. 
        Steps to reproduce:
           - Given I am authorized user 
           - Send GET request to /zip-codes endpoint

        Expected result: Get response code "200 OK" and get all available zip codes in the application for now
        Actual result: Get response code "201 Created" and get all available zip codes in the application for now

        2. Duplicated zip codes are added to the app. 
        Steps to reproduce:
           - Given I am authorized user 
           - send POST request to /zip-codes/expand endpoint
             with Request body that inclides list of zip codes 
             has duplications for already used zip codes

        Expected result: Get response code "201 Created", zip codes from request body are added 
                         to available zip codes of application And there ARE NO duplications 
                         between available zip codes and already used zip codes 
        Actual result: Get response code "201 Created", zip codes from request body are added 
                       to available zip codes of application And there ARE duplications 
                       between available zip codes and already used zip codes 
        */
    }
}