using ApiTestingSolution.Enums;

namespace ApiTestingSolution.Authenticators
{
    public class ReadScopeHandler : BaseTokenHandler
    {
        public ReadScopeHandler(HttpClient httpClient) : base(httpClient, Scope.Read)
        { }
    }
}
