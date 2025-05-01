using System.Net.Sockets;
using LightResults;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.Caching.Memory;
using Mycelium.Features.Java.Packets;

namespace Mycelium.Features.Java;

/// <summary>
/// Represents a simple Minecraft Java edition client.
/// </summary>
/// <param name="logger">The <see cref="ILogger"/> used to log actions.</param>
/// <param name="cache">The <see cref="IMemoryCache"/> used for caching <see cref="JavaResponse"/>s.</param>
/// <param name="factory">The <see cref="SocketFactory"/> used for creating <see cref="Socket"/>s.</param>
internal sealed class JavaClient(ILogger<JavaClient> logger, IMemoryCache cache, SocketFactory factory)
{
    // https://github.com/dotnet/aspnetcore/blob/c22a8530ee463bf3534ce5fc54f991e8ab1e9ee0/src/Servers/Kestrel/Transport.Sockets/src/SocketConnectionListener.cs#L31.
    private readonly SocketConnectionContextFactory connectionContextFactory = new(new SocketConnectionFactoryOptions(), logger);

    /// <summary>
    /// Performs a status request to the given input address.
    /// </summary>
    /// <param name="input">The input address to request status from.</param>
    /// <param name="token">A <see cref="CancellationToken"/> that can be used to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="Result"/> containing the <see cref="JavaResponse"/>.</returns>
    public async Task<Result<JavaResponse>> RequestStatusAsync(string input, CancellationToken token)
    {
        if (!Address.TryParse(input, out var address))
        {
            return Result.Failure<JavaResponse>("Invalid address.");
        }

        if (cache.TryGetValue(input, out JavaResponse? response))
        {
            return Result.Success(response!);
        }

        var connecting = await factory.ConnectAsync(address, SocketType.Stream, ProtocolType.Tcp, token);

        if (!connecting.IsSuccess(out var socket))
        {
            return connecting.AsFailure<JavaResponse>();
        }

        await using var connection = connectionContextFactory.Create(socket);

        await StatusRequestPacket.WriteAsync(connection.Transport.Output, address.First, address.Port, token);

        var reading = await StatusResponsePacket.ReadAsync(connection.Transport.Input, token);

        if (!reading.IsSuccess(out var status))
        {
            return reading.AsFailure<JavaResponse>();
        }

        var result = JavaResponse.TryCreate(status, out response)
            ? Result.Success(cache.Set(input, response))
            : Result.Failure<JavaResponse>("Could not read status response.");

        connection.Transport.Input.AdvanceTo(status.Start, status.End);

        return result;
    }
}