using ApiTestingSolution.Services;
using System.Net;
using ApiTestingSolution.Helpers;
using NUnit.Framework.Legacy;
using Allure.NUnit.Attributes;
using Allure.NUnit;
using Microsoft.Extensions.DependencyInjection;

namespace ApiTestingSolution.Tests
{
    [AllureNUnit]
    [AllureSuite("Tests - ZipCode controller")]
    public class ZipCodeControllerTests
    {
        private ZipCodeControllerService _zipCodeService;

        [OneTimeSetUp]
        public void Setup()
        {
            _zipCodeService = TestSetup.ServiceProvider.GetRequiredService<ZipCodeControllerService>();
        }

        [Test, Order(1)]
        [AllureFeature("GetAvailableZipCodesTest")]
        [AllureStory("Validate get all available zip codes from the app")]
        public async Task GetAvailableZipCodesTest()
        {
            var response = await _zipCodeService.GetAvailableZipCodesAsync();
            var codes = await JsonHelper.DeserializeJsonContentAsync<List<string>>(response);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Status code should be 200 OK");
                CollectionAssert.IsNotEmpty(codes, "Content should contain some ZIP codes");
            });
        }

        [Test, Order(1)]
        [AllureFeature("ExpandAvailableZipCodesTest")]
        [AllureStory("Validate adding correct zip code to the app")]
        public async Task ExpandAvailableZipCodesTest()
        {
            var zipCodes = new List<string>
            {
                RandomHelper.GetRandomString(5),
                RandomHelper.GetRandomString(8)
            };

            var response = await _zipCodeService.ExpendAvailableZipCodesAsync(zipCodes);
            var actualCodes = await JsonHelper.DeserializeJsonContentAsync<List<string>>(response);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created), "Status code should be 201 Created");
                CollectionAssert.IsSubsetOf(zipCodes, actualCodes, "The application doesn't contain added ZIP codes");
            });
        }

        [Test]
        [AllureFeature("ExpandAvailableZipCodesTest")]
        [AllureStory("Validate adding incorrect zip code to the app")]
        public async Task ExpandAvailableZipCodesByDuplicationsTest()
        {
            var response = await _zipCodeService.GetAvailableZipCodesAsync();
            var actualCodes = await JsonHelper.DeserializeJsonContentAsync<List<string>>(response);
            var zipCodes = new List<string>(actualCodes)
        {
            actualCodes.First()
        };

            response = await _zipCodeService.ExpendAvailableZipCodesAsync(zipCodes);
            actualCodes = await JsonHelper.DeserializeJsonContentAsync<List<string>>(response);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created), "Status code should be 201 Created");
                Assert.That(actualCodes, Is.Unique, "The list of the app ZIP codes contains duplications");
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