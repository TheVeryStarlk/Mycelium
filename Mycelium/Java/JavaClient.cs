using System.Net;
using System.Net.Sockets;
using LightResults;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.Caching.Memory;

namespace Mycelium.Java;

internal sealed class JavaClient(ILogger<JavaClient> logger, IMemoryCache cache)
{
    private readonly SocketConnectionContextFactory factory = new(new SocketConnectionFactoryOptions(), logger);

    public async Task<Result<StatusResponse>> RequestStatusAsync(string input)
    {
        if (!Address.TryParse(input, out var address))
        {
            return Result.Failure<StatusResponse>("Invalid address.");
        }

        if (cache.TryGetValue(input, out StatusResponse? response))
        {
            return Result.Success(response!);
        }

        var result = await ConnectAsync(address.First, address.Port);
        return !result.IsSuccess(out _) ? result.AsFailure<StatusResponse>() : Result.Success(StatusResponse.Create([]));
    }

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

        return addresses.Length is 0 ? Result.Failure<IPAddress>("Failed to resolve hostname.") : addresses[0];
    }
}