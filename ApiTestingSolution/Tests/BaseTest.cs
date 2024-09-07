using ApiTestingSolution.ClientFactory;
using ApiTestingSolution.Helpers;
using ApiTestingSolution.Logging;
using ApiTestingSolution.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ApiTestingSolution.Tests
{
    public abstract class BaseTest
    {
        protected static ServiceProvider ServiceProvider { get; private set; }
        protected static ZipCodeControllerService ZipCodeService { get; private set; }
        protected static UserControllerService UserControllerService { get; private set; }

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            var serviceCollection = new ServiceCollection();
            Logger.Info("Registering dependencies");
            serviceCollection.AddApiClients();
            ServiceProvider = serviceCollection.BuildServiceProvider();            
            ZipCodeService = ServiceProvider.GetRequiredService<ZipCodeControllerService>();
            UserControllerService = ServiceProvider.GetRequiredService<UserControllerService>();
            UserHelpers.Initialize(ZipCodeService);
            Logger.Info("Global setup completed");
        }

        [OneTimeTearDown]
        public void GlobalTeardown()
        {           
            ServiceProvider?.Dispose();
            Logger.Info("Global teardown completed");
        }
    }
}
