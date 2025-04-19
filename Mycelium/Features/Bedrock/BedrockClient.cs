using System.Net.Sockets;
using LightResults;
using Microsoft.Extensions.Caching.Memory;
using Mycelium.Features.Bedrock.Packets;

namespace Mycelium.Features.Bedrock;

internal sealed class BedrockClient(ILogger<BedrockClient> logger, IMemoryCache cache)
{
    private const string Prefix = "Bedrock";

    public async Task<Result<StatusResponse>> RequestStatusAsync(string input)
    {
        if (!Address.TryParse(input, out var address))
        {
            return Result.Failure<StatusResponse>("Invalid address.");
        }

        if (cache.TryGetValue($"{Prefix}{input}", out StatusResponse? response))
        {
            return Result.Success(response!);
        }

        var connecting = await ConnectAsync(address.First, address.Port);

        if (!connecting.IsSuccess(out var connection))
        {
            return connecting.AsFailure<StatusResponse>();
        }

        await connection.SendAsync(RakNet.UnconnectedPingPacket.AsMemory());

        var reading = await UnconnectedPongPacket.ReadAsync(connection);

        connection.Dispose();

        if (!reading.IsSuccess(out var status))
        {
            return reading.AsFailure<StatusResponse>();
        }

        return StatusResponse.TryCreate(Edition.Bedrock, status, out response)
            ? Result.Success(cache.Set($"{Prefix}{input}", response))
            : Result.Failure<StatusResponse>("Could not read status response.");
    }

    private async Task<Result<Socket>> ConnectAsync(string address, ushort port)
    {
        var resolving = await HostUtility.ResolveHostAsync(address);

        if (!resolving.IsSuccess(out var host))
        {
            return resolving.AsFailure<Socket>();
        }

        var socket = new Socket(host.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

        try
        {
            // This "doesn't" actually create a connection.
            await socket.ConnectAsync(host, port);
        }
        catch (Exception exception)
        {
            logger.LogDebug(exception, "An exception occurred while connecting to the server");
            return Result.Failure<Socket>("Could not connect to the server.");
        }

        return socket;
    }
}