using Microsoft.Extensions.DependencyInjection;
using Mycelium.Bedrock;

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