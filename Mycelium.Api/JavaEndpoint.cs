using Microsoft.AspNetCore.Http.HttpResults;
using Mycelium.Java;

namespace Mycelium.Api;

internal static class JavaEndpoint
{
    public static void MapJava(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(
            "/java/{address}",
            async Task<Ok<JavaResponse>> (string address, CancellationToken token, JavaClient client) =>
            {
                var status = await client.RequestStatusAsync(address, token);
                return TypedResults.Ok(status);
            });
    }
}