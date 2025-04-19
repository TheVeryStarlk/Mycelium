using System.Net.Sockets;
using LightResults;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.Caching.Memory;
using Mycelium.Features.Java.Packets;

namespace Mycelium.Features.Java;

/// <summary>
/// Represents a simple Minecraft Java edition client.
/// </summary>
/// <param name="logger">The <see cref="ILogger"/> used to log actions.</param>
/// <param name="cache">The <see cref="IMemoryCache"/> used for caching <see cref="StatusResponse"/>s.</param>
internal sealed class JavaClient(ILogger<JavaClient> logger, IMemoryCache cache)
{
    // https://github.com/dotnet/aspnetcore/blob/c22a8530ee463bf3534ce5fc54f991e8ab1e9ee0/src/Servers/Kestrel/Transport.Sockets/src/SocketConnectionListener.cs#L31.
    private readonly SocketConnectionContextFactory factory = new(new SocketConnectionFactoryOptions(), logger);

    /// <summary>
    /// Performs a status request to the given input address.
    /// </summary>
    /// <param name="input">The input address to request status from.</param>
    /// <returns>A <see cref="Result"/> containing the <see cref="StatusResponse"/>.</returns>
    public async Task<Result<StatusResponse>> RequestStatusAsync(string input)
    {
        if (!Address.TryParse(input, out var address))
        {
            return Result.Failure<StatusResponse>("Invalid address.");
        }

        if (cache.TryGetValue($"{Edition.Java}{input}", out StatusResponse? response))
        {
            return Result.Success(response!);
        }

        var connecting = await ConnectAsync(address.First, address.Port);

        if (!connecting.IsSuccess(out var connection))
        {
            return connecting.AsFailure<StatusResponse>();
        }

        await StatusRequestPacket.WriteAsync(connection.Transport.Output, address.First, address.Port);

        var reading = await StatusResponsePacket.ReadAsync(connection.Transport.Input);

        // Disposes of the socket as well.
        connection.Abort();

        if (!reading.IsSuccess(out var status))
        {
            return reading.AsFailure<StatusResponse>();
        }

        return StatusResponse.TryCreate(Edition.Java, status, out response)
            ? Result.Success(cache.Set($"{Edition.Java}{input}", response))
            : Result.Failure<StatusResponse>("Could not read status response.");
    }

    /// <summary>
    /// Creates a TCP <see cref="ConnectionContext"/> to the given address and port.
    /// </summary>
    /// <param name="address">The address to connect to.</param>
    /// <param name="port">The port to connect to.</param>
    /// <returns>A <see cref="Result"/> containing the TCP <see cref="ConnectionContext"/>.</returns>
    private async Task<Result<ConnectionContext>> ConnectAsync(string address, ushort port)
    {
        var resolving = await HostUtility.ResolveHostAsync(address);

        if (!resolving.IsSuccess(out var host))
        {
            return resolving.AsFailure<ConnectionContext>();
        }

        var socket = new Socket(host.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            await socket.ConnectAsync(host, port);
        }
        catch (Exception exception)
        {
            logger.LogDebug(exception, "An exception occurred while connecting to the server");
            return Result.Failure<ConnectionContext>("Could not connect to the server.");
        }

        return factory.Create(socket);
    }
}