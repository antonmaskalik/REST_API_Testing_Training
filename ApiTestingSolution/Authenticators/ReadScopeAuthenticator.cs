namespace ApiTestingSolution.Authenticators
{
    public class ReadScopeAuthenticator : BaseTokenAuthenticator
    {
        public ReadScopeAuthenticator(string clientId, string clientSecret, string baseUrl)
        : base(clientId, clientSecret, baseUrl, Enums.Scope.Read)
        {
        }
    }
}
