using ApiTestingSolution.Authenticators;
using ApiTestingSolution.Constants;
using ApiTestingSolution.Logging;
using RestSharp;

namespace ApiTestingSolution.ClientFactory
{
    public class ApiReadClient
    {
        private static RestClient? _readClient;

        public static RestClient GetRestClient()
        {
            if (_readClient == null)
            {
                var options = new RestClientOptions(GlobalConstants.BaseUrl)
                {
                    Authenticator = new ReadScopeAuthenticator(GlobalConstants.UserName,
                    GlobalConstants.Password, GlobalConstants.BaseUrl),
                };
                _readClient = new RestClient(options);
                Logger.Info("Read client is created");
            }

            return _readClient;
        }
    }
}
