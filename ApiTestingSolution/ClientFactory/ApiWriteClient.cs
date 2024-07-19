using ApiTestingSolution.Authenticators;
using ApiTestingSolution.Constants;
using ApiTestingSolution.Logging;
using RestSharp;

namespace ApiTestingSolution.ClientFactory
{
    public class ApiWriteClient 
    {
        private static RestClient? _writeClient;

        public static RestClient GetRestClient()
        {
            if (_writeClient == null)
            {
                var options = new RestClientOptions(GlobalConstants.BaseUrl)
                {
                    Authenticator = new WriteScopeAuthenticator(GlobalConstants.UserName, 
                    GlobalConstants.Password, GlobalConstants.BaseUrl),
                };
                _writeClient = new RestClient(options);
                Logger.Info("The write client is created");
            }

            return _writeClient;
        }
    }
}
