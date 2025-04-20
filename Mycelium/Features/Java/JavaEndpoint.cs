using Microsoft.AspNetCore.Http.HttpResults;

namespace Mycelium.Features.Java;

/// <summary>
/// Handles registering and mapping all Minecraft Java edition related services and endpoints.
/// </summary>
internal static class JavaEndpoint
{
    /// <summary>
    /// Registers all Minecraft Java edition services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    public static void AddJava(this IServiceCollection services)
    {
        services.AddSingleton<SocketFactory>();
        services.AddSingleton<JavaClient>();
    }

    /// <summary>
    /// Maps all Minecraft Java edition endpoints.
    /// </summary>
    /// <param name="application">The <see cref="WebApplication"/> to add the endpoints to.</param>
    public static void MapJava(this WebApplication application)
    {
        // To add ping as well later.
        var group = application.MapGroup($"/{Edition.Java}");

        group.MapGet(
            "/status/{input}",
            async Task<Results<Ok<JavaResponse>, ProblemHttpResult>> (string input, JavaClient client, CancellationToken token) =>
            {
                var result = await client.RequestStatusAsync(input, token);
                return result.ToTypedResults();
            });
    }
}