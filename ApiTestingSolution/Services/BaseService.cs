using ApiTestingSolution.ClientFactory;
using ApiTestingSolution.Logging;
using ApiTestingSolution.Enums;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Text;
using ApiTestingSolution.Constants;
using Newtonsoft.Json.Converters;
using System.Net.Http.Headers;

namespace ApiTestingSolution.Services
{
    public abstract class BaseService
    {
        protected readonly IApiClient _readClient;
        protected readonly IApiClient _writeClient;

        protected BaseService(IReadApiClient readClient, IWriteApiClient writeClient)
        {
            _readClient = readClient;
            _writeClient = writeClient;
        }

        protected HttpRequestMessage CreateRequest(string endpoint, HttpMethod method, object? body = null)
        {
            var request = new HttpRequestMessage(method, GlobalConstants.BaseUrl + endpoint);
            Logger.Info($"{method} request to {request.RequestUri} is created");

            if (body != null)
            {
                var jsonBody = JsonConvert.SerializeObject(body);                
                request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                Logger.Info($"Body of the request is: {SanitizeBody(jsonBody)}");
            }

            return request;
        }

        protected async Task<HttpResponseMessage> ExecuteRequestAsync(HttpRequestMessage request, Scope scope)
        {
            var client = scope == Scope.Read ? _readClient : _writeClient;
            Logger.Info($"Sending request to {request.RequestUri} with method {request.Method}");
            var response = await client.SendAsync(request);

            var responseContent = await response.Content.ReadAsStringAsync();
            Logger.Info($"Response from {request.RequestUri} is received. " +
                $"Status Code: {(int)response.StatusCode}, " +
                $"Response Body: {SanitizeBody(responseContent)}");

            return response;
        }

        private static string SanitizeBody(string body)
        {
            if (string.IsNullOrEmpty(body))
                return body;

            var sanitizedBody = body;
            sanitizedBody = Regex.Replace(sanitizedBody, "\"password\":\"[^\"]+\"", "\"password\":\"****\"");

            return sanitizedBody;
        }
    }
}
