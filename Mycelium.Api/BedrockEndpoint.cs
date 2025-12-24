using Microsoft.AspNetCore.Http.HttpResults;
using Mycelium.Bedrock;

namespace Mycelium.Api;

internal static class BedrockEndpoint
{
    public static void MapBedrock(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(
            "/bedrock/{address}",
            async Task<Ok<BedrockResponse>> (string address, CancellationToken token, BedrockClient client) =>
            {
                var status = await client.RequestStatusAsync(address, token);
                return TypedResults.Ok(status);
            });
    }
}