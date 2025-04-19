using Microsoft.AspNetCore.Http.HttpResults;

namespace Mycelium.Features.Bedrock;

/// <summary>
/// Handles registering and mapping all Minecraft Bedrock edition related services and endpoints.
/// </summary>
internal static class BedrockEndpoint
{
    /// <summary>
    /// Registers all Minecraft Bedrock edition services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    public static void AddBedrock(this IServiceCollection services)
    {
        services.AddSingleton<BedrockClient>();
    }

    /// <summary>
    /// Maps all Minecraft Bedrock edition endpoints.
    /// </summary>
    /// <param name="application">The <see cref="WebApplication"/> to add the endpoints to.</param>
    public static void MapBedrock(this WebApplication application)
    {
        // To add ping as well later.
        var group = application.MapGroup($"/{Edition.Bedrock}");

        group.MapGet(
            "/status/{input}",
            async Task<Results<Ok<StatusResponse>, ProblemHttpResult>> (string input, BedrockClient client) =>
            {
                var result = await client.RequestStatusAsync(input);
                return result.ToTypedResults();
            });
    }
}