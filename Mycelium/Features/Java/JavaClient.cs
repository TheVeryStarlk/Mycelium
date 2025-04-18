using System.Net;
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

        if (cache.TryGetValue($"Java{input}", out StatusResponse? response))
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

        if (!reading.IsSuccess(out var status))
        {
            return reading.AsFailure<StatusResponse>();
        }

        return StatusResponse.TryCreate(status, out response)
            ? Result.Success(cache.Set($"Java{input}", response))
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
        var result = await ResolveHostAsync(address);

        if (!result.IsSuccess(out var host))
        {
            return result.AsFailure<ConnectionContext>();
        }

        // Is disposed by the connection context.
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

    /// <summary>
    /// Resolves DNS host of a given address.
    /// </summary>
    /// <param name="address">The address to resolve DNS from.</param>
    /// <returns>A <see cref="Result"/> containing the DNS resolved <see cref="IPAddress"/>.</returns>
    private async Task<Result<IPAddress>> ResolveHostAsync(string address)
    {
        var addresses = Array.Empty<IPAddress>();

        try
        {
            addresses = await Dns.GetHostAddressesAsync(address);
        }
        catch (Exception exception)
        {
            logger.LogDebug(exception, "An exception occurred while getting host addresses");
        }

        // I think one is enough. We could try to take all resolved addresses and loop into each one till we get a valid connection.
        return addresses.Length is 0 ? Result.Failure<IPAddress>("Failed to resolve hostname.") : addresses[0];
    }
}