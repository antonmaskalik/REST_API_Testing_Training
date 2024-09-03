namespace ApiTestingSolution.ClientFactory
{
    public interface IApiClient
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
    }
}
