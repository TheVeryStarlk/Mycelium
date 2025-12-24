using Microsoft.Extensions.DependencyInjection;
using Mycelium.Editions;
using Mycelium.Editions.Bedrock;

namespace Mycelium.Hosting;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMycelium(Action<MyceliumOptions> action)
        {
            services.Configure(action);

            services.AddSingleton<ISocketFactory, SocketFactory>();
            
            services.AddTransient<BedrockClient>();
            
            return services;
        }
    }
}