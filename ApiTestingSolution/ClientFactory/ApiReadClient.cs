using ApiTestingSolution.Logging;

namespace ApiTestingSolution.ClientFactory
{
    public class ApiReadClient : IReadApiClient
    {
        private readonly HttpClient _httpClient;

        public ApiReadClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            Logger.Info($"Sending read request to {request.RequestUri}");
            var response = await _httpClient.SendAsync(request);
            Logger.Info($"Received read response with status code {response.StatusCode}");

            return response;
        }
    }
}
