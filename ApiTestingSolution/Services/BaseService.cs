using ApiTestingSolution.ClientFactory;
using ApiTestingSolution.Logging;
using ApiTestingSolution.Enums;
using RestSharp;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace ApiTestingSolution.Services
{
    public abstract class BaseService
    {
        protected static RestRequest CreateRequest(string endpoint, Method method, object body = null)
        {
            var request = new RestRequest(endpoint, method);
            Logger.Info($"{method} request to /{request.Resource} is created");

            if (body != null)
            {
                request.AddJsonBody(body);
                Logger.Info($"Body of the request is: {SanitizeBody(JsonConvert.SerializeObject(body).ToString())}");
            }
          
            return request;
        }

        protected static RestResponse ExecuteRequest(RestRequest request, Scope scope)
        {
            var client = scope == Scope.Read 
                ? ApiReadClient.GetRestClient() 
                : ApiWriteClient.GetRestClient();
           
            var response = client.Execute(request);
            Logger.Info($"Response from /{request.Resource} is received. " +
                $"Status Code: {response.StatusCode}, " +
                $"Response Body: {SanitizeBody(response.Content)}");

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
