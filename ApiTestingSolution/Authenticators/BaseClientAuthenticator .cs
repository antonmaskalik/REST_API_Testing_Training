using ApiTestingSolution.Enums;
using ApiTestingSolution.Logging;
using ApiTestingSolution.Models;
using RestSharp;
using RestSharp.Authenticators;

namespace ApiTestingSolution.Authenticators
{
    public abstract class BaseTokenAuthenticator : AuthenticatorBase
    {
        private static RestClient? _client;
        
        protected RestClientOptions Options { get; }
        protected string Scope { get; }

        protected BaseTokenAuthenticator(string userName, string password, string baseUrl, Scope scope) : base(string.Empty)
        {
            Scope = scope.ToString().ToLower();
            Options = new RestClientOptions(baseUrl)
            {
                Authenticator = new HttpBasicAuthenticator(userName, password),
            };

            _client = new RestClient(Options);
        }

        protected override async ValueTask<Parameter> GetAuthenticationParameter(string accessToken)
        {
            Token = string.IsNullOrEmpty(Token) ? await GetToken() : Token;
            return new HeaderParameter(KnownHeaders.Authorization, Token);
        }

        private async Task<string> GetToken()
        {            
            var request = new RestRequest("oauth/token");
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("scope", Scope);
            var response = await _client!.PostAsync<TokenResponse>(request);
            Logger.Info("The client's token is recived");

            return $"{response!.TokenType} {response!.AccessToken}";
        }
    }
}