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
    /// <param name="application">The <see cref="WebApplication"/> to add the endpoints to.</param>
    public static void MapJava(this WebApplication application)
    {
        // To add ping as well later.
        var group = application.MapGroup("/java");

        group.MapGet(
            "/status/{input}",
            async Task<Results<Ok<JavaResponse>, ProblemHttpResult>> (string input, JavaClient client, CancellationToken token) =>
            {
                var result = await client.RequestStatusAsync(input, token);
                return result.ToTypedResults();
            });
    }
}