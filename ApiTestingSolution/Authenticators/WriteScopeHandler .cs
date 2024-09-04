using ApiTestingSolution.Enums;

namespace ApiTestingSolution.Authenticators
{
    public class WriteScopeHandler : BaseTokenHandler
    {
        public WriteScopeHandler(HttpClient httpClient) : base(httpClient, Scope.Write)
        { }
    }
}