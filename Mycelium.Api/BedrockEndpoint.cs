using Microsoft.AspNetCore.Http.HttpResults;
using Mycelium.Bedrock;

namespace Mycelium.Api;

internal static class BedrockEndpoint
{
    public static void MapBedrock(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(
            "/bedrock/{input}",
            async Task<Results<Ok<BedrockResponse>, BadRequest>> (string input, CancellationToken token, BedrockClient client) =>
            {
                if (!Address.TryParse(input, out var address))
                {
                    return TypedResults.BadRequest();
                }

                var status = await client.RequestStatusAsync(address, token);

                if (!BedrockResponse.TryCreate(status, out var response))
                {
                    return TypedResults.BadRequest();
                }

                return TypedResults.Ok(response);
            });
    }
}