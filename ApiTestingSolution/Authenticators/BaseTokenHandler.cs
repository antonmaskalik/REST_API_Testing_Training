using ApiTestingSolution.Constants;
using ApiTestingSolution.Enums;
using ApiTestingSolution.Logging;
using ApiTestingSolution.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace ApiTestingSolution.Authenticators
{
    public abstract class BaseTokenHandler : DelegatingHandler
    {
        private readonly HttpClient _httpClient;
        private readonly string _scope;
        private string? _token;

        protected BaseTokenHandler(HttpClient httpClient, Scope scope)
        {
            _httpClient = httpClient;
            _scope = scope.ToString().ToLower();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _token = string.IsNullOrEmpty(_token) ? await GetTokenAsync() : _token;
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token.Split(' ')[1]);

            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<string> GetTokenAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "oauth/token");
            var parameters = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "scope", _scope }
            };

            request.Content = new FormUrlEncodedContent(parameters);
            _httpClient.BaseAddress = new Uri(GlobalConstants.BaseUrl);
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($"{GlobalConstants.UserName}:{GlobalConstants.Password}")));
            var response = await _httpClient.SendAsync(request);
            Logger.Info("The client's token is recived");
            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
            Logger.Info("Token received successfully");

            return $"{tokenResponse.TokenType} {tokenResponse.AccessToken}";
        }
    }
}