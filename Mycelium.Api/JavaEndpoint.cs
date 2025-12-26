using Microsoft.AspNetCore.Http.HttpResults;
using Mycelium.Java;

namespace Mycelium.Api;

internal static class JavaEndpoint
{
    public static void MapJava(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(
            "/java/{input}",
            async Task<Results<Ok<JavaResponse>, BadRequest>> (string input, CancellationToken token, JavaClient client) =>
            {
                if (!Address.TryParse(input, out var address))
                {
                    return TypedResults.BadRequest();
                }
                
                var status = await client.RequestStatusAsync(address, token);

                if (!JavaResponse.TryCreate(status, out var response))
                {
                    return TypedResults.BadRequest();
                }
                
                return TypedResults.Ok(response);
            });
    }
}