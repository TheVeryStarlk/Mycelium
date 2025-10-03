using Microsoft.AspNetCore.Http.HttpResults;

namespace Mycelium.Features.Java;

/// <summary>
/// Handles registering and mapping all Minecraft Java edition's endpoints.
/// </summary>
internal static class JavaEndpoint
{
    /// <summary>
    /// Maps all Minecraft Java edition endpoints.
    /// </summary>
    /// <param name="route">The <see cref="IEndpointRouteBuilder"/> to add the endpoints to.</param>
    public static void MapJava(this IEndpointRouteBuilder route)
    {
        // To add ping as well later.
        var group = route.MapGroup("/java");

        group.MapGet(
            "/status/{input}",
            async Task<Results<Ok<JavaResponse>, ProblemHttpResult>> (string input, JavaClient client, CancellationToken token) =>
            {
                var result = await client.RequestStatusAsync(input, token);
                return result.ToTypedResults();
            });
    }
}