namespace ApiTestingSolution.Authenticators
{
    public class WriteScopeAuthenticator : BaseTokenAuthenticator
    {
        public WriteScopeAuthenticator(string clientId, string clientSecret, string baseUrl)
        : base(clientId, clientSecret, baseUrl, Enums.Scope.Write)
        {
        }
    }
}
