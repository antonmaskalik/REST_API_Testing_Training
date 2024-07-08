using ApiTestingSolution.Authenticators;
using ApiTestingSolution.Constants;
using RestSharp;

namespace ApiTestingSolution.ClientFactory
{
    public class ApiReadClient
    {
        private static RestClient? _writeClient;

        public static RestClient GetRestClient()
        {
            if (_writeClient == null)
            {
                var options = new RestClientOptions(GlobalConstants.BaseUrl)
                {
                    Authenticator = new ReadScopeAuthenticator(GlobalConstants.UserName, 
                    GlobalConstants.Password, GlobalConstants.BaseUrl),
                };
                _writeClient = new RestClient(options);
            }

            return _writeClient;
        }
    }
}
