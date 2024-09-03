using ApiTestingSolution.Logging;

namespace ApiTestingSolution.ClientFactory
{
    public class ApiWriteClient : IWriteApiClient
    {
        private readonly HttpClient _httpClient;

        public ApiWriteClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            Logger.Info($"Sending write request to {request.RequestUri}");
            var response = await _httpClient.SendAsync(request);
            Logger.Info($"Received write response with status code {response.StatusCode}");

            return response;
        }
    }
}
