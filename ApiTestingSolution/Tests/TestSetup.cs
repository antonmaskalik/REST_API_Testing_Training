using ApiTestingSolution.ClientFactory;
using ApiTestingSolution.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace ApiTestingSolution.Tests
{
    [SetUpFixture]
    public class TestSetup
    {
        public static ServiceProvider ServiceProvider { get; private set; }

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            var serviceCollection = new ServiceCollection();
            Logger.Info("Registering dependencies");
            serviceCollection.AddApiClients();
            ServiceProvider = serviceCollection.BuildServiceProvider();
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
