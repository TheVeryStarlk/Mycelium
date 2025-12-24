using Microsoft.Extensions.DependencyInjection;
using Mycelium.Bedrock;
using Mycelium.Java;

namespace Mycelium;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMycelium(this IServiceCollection services)
    {
        services.AddSingleton<ISocketFactory, SocketFactory>();

        services.AddTransient<BedrockClient>();
        services.AddTransient<JavaClient>();

        return services;
    }
}