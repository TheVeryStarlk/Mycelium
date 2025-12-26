using Microsoft.Extensions.DependencyInjection;
using Mycelium.Bedrock;
using Mycelium.Java;

namespace Mycelium;

/// <summary>
/// Contains extension method(s) to <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Mycelium to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddMycelium(this IServiceCollection services)
    {
        services.AddSingleton<ISocketFactory, SocketFactory>();

        services.AddTransient<BedrockClient>();
        services.AddTransient<JavaClient>();

        return services;
    }
}