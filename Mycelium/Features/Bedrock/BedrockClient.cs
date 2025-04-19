using LightResults;
using Microsoft.Extensions.Caching.Memory;
using Mycelium.Features.Bedrock.Packets;

namespace Mycelium.Features.Bedrock;

/// <summary>
/// Represents a simple Minecraft Bedrock edition client.
/// </summary>
/// <param name="logger">The <see cref="ILogger"/> used to log actions.</param>
/// <param name="cache">The <see cref="IMemoryCache"/> used for caching <see cref="StatusResponse"/>s.</param>
internal sealed class BedrockClient(IMemoryCache cache, SocketFactory factory)
{
    /// <summary>
    /// Performs a status request to the given input address.
    /// </summary>
    /// <param name="input">The input address to request status from.</param>
    /// <param name="token">A <see cref="CancellationToken"/> that can be used to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="Result"/> containing the <see cref="StatusResponse"/>.</returns>
    public async Task<Result<StatusResponse>> RequestStatusAsync(string input, CancellationToken token)
    {
        if (!Address.TryParse(input, out var address))
        {
            return Result.Failure<StatusResponse>("Invalid address.");
        }

        if (cache.TryGetValue($"{Edition.Bedrock}{input}", out StatusResponse? response))
        {
            return Result.Success(response!);
        }

        var connecting = await factory.ConnectAsync(Edition.Bedrock, address.First, address.Port, token);

        if (!connecting.IsSuccess(out var socket))
        {
            return connecting.AsFailure<StatusResponse>();
        }

        await socket.SendAsync(RakNet.UnconnectedPingPacket.AsMemory(), token);

        var reading = await UnconnectedPongPacket.ReadAsync(socket, token);

        socket.Dispose();

        if (!reading.IsSuccess(out var status))
        {
            return reading.AsFailure<StatusResponse>();
        }

        return StatusResponse.TryCreate(Edition.Bedrock, status, out response)
            ? Result.Success(cache.Set($"{Edition.Bedrock}{input}", response))
            : Result.Failure<StatusResponse>("Could not read status response.");
    }
}