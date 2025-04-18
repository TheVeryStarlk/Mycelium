using Microsoft.AspNetCore.Http.HttpResults;

namespace Mycelium.Java;

/// <summary>
/// Handles registering and mapping all Minecraft Java related services and endpoints.
/// </summary>
internal static class JavaEndpoint
{
    /// <summary>
    /// Registers all Minecraft Java services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    public static void AddJava(this IServiceCollection services)
    {
        services.AddSingleton<JavaClient>();
    }

    /// <summary>
    /// Maps all Minecraft Java endpoints.
    /// </summary>
    /// <param name="application">The <see cref="WebApplication"/> to add the endpoints to.</param>
    public static void MapJava(this WebApplication application)
    {
        // To add ping as well later.
        var group = application.MapGroup("/java");

        group.MapGet(
            "/status/{address}",
            async Task<Results<Ok<StatusResponse>, ProblemHttpResult>> (string address, JavaClient client) =>
            {
                var result = await client.RequestStatusAsync(address);
                return result.ToTypedResults();
            });
    }
}