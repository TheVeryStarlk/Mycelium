using Microsoft.Extensions.DependencyInjection;
using Mycelium.Clients.Bedrock;
using Mycelium.Editions;

namespace Mycelium;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMycelium(this IServiceCollection services)
    {
        services.AddSingleton<ISocketFactory, SocketFactory>();
        services.AddTransient<BedrockClient>();

        return services;
    }
}