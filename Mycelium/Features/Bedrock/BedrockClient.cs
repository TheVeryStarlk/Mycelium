using System.Net.Sockets;
using LightResults;
using Microsoft.Extensions.Caching.Memory;
using Mycelium.Features.Bedrock.Packets;

namespace Mycelium.Features.Bedrock;

/// <summary>
/// Represents a simple Minecraft Bedrock edition client.
/// </summary>
/// <param name="cache">The <see cref="IMemoryCache"/> used for caching <see cref="BedrockResponse"/>s.</param>
/// <param name="factory">The <see cref="BedrockSocketFactory"/> used for creating <see cref="Socket"/>s.</param>
internal sealed class BedrockClient(IMemoryCache cache, BedrockSocketFactory factory)
{
    /// <summary>
    /// Performs a status request to the given input address.
    /// </summary>
    /// <param name="input">The input address to request status from.</param>
    /// <param name="token">A <see cref="CancellationToken"/> that can be used to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="Result"/> containing the <see cref="BedrockResponse"/>.</returns>
    public async Task<Result<BedrockResponse>> RequestStatusAsync(string input, CancellationToken token)
    {
        if (!Address.TryParse(input, out var address))
        {
            return Result.Failure<BedrockResponse>("Invalid address.");
        }

        if (cache.TryGetValue($"Bedrock{input}", out BedrockResponse? response))
        {
            return Result.Success(response!);
        }

        var connecting = await factory.ConnectAsync(address.First, address.Port, token);

        if (!connecting.IsSuccess(out var socket))
        {
            return connecting.AsFailure<BedrockResponse>();
        }

        var writing = await UnconnectedPingPacket.WriteAsync(socket);

        if (!writing.IsSuccess())
        {
            return writing.AsFailure<BedrockResponse>();
        }

        var reading = await UnconnectedPongPacket.ReadAsync(socket, token);

        socket.Dispose();

        if (!reading.IsSuccess(out var status))
        {
            return reading.AsFailure<BedrockResponse>();
        }

        return BedrockResponse.TryCreate(status, out response)
            ? Result.Success(cache.Set($"Bedrock{input}", response))
            : Result.Failure<BedrockResponse>("Could not read status response.");
    }
}