using ApiTestingSolution.Authenticators;
using ApiTestingSolution.Constants;
using ApiTestingSolution.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ApiTestingSolution.ClientFactory
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiClients(this IServiceCollection services)
        {
            services.AddHttpClient<IReadApiClient, ApiReadClient>(client =>
            {
                client.BaseAddress = new Uri(GlobalConstants.BaseUrl);
            }).AddHttpMessageHandler<ReadScopeHandler>();
            services.AddHttpClient<IWriteApiClient, ApiWriteClient>(client =>
            {
                client.BaseAddress = new Uri(GlobalConstants.BaseUrl);
            }).AddHttpMessageHandler<WriteScopeHandler>();

            services.AddTransient<ReadScopeHandler>();
            services.AddTransient<WriteScopeHandler>();
            services.AddTransient<ZipCodeControllerService>();
            services.AddTransient<UserControllerService>();

            return services;
        }
    }
}
