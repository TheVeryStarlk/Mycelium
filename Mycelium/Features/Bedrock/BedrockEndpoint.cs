using Microsoft.AspNetCore.Http.HttpResults;

namespace Mycelium.Features.Bedrock;

/// <summary>
/// Handles registering and mapping all Minecraft Bedrock edition's endpoints.
/// </summary>
internal static class BedrockEndpoint
{
    /// <summary>
    /// Maps all Minecraft Bedrock edition endpoints.
    /// </summary>
    /// <param name="route">The <see cref="IEndpointRouteBuilder"/> to add the endpoints to.</param>
    public static void MapBedrock(this IEndpointRouteBuilder route)
    {
        var group = route.MapGroup("/bedrock");

        group.MapGet(
            "/status/{input}",
            async Task<Results<Ok<BedrockResponse>, ProblemHttpResult>> (string input, BedrockClient client, CancellationToken token) =>
            {
                var result = await client.RequestStatusAsync(input, token);
                return result.ToTypedResults();
            });
    }
}