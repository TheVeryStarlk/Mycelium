using Microsoft.Extensions.DependencyInjection;
using Mycelium.Editions;
using Mycelium.Editions.Bedrock;

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