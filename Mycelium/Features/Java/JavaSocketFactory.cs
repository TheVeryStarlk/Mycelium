using System.Net.Sockets;
using LightResults;

namespace Mycelium.Features.Java;

/// <summary>
/// A factory for creating <see cref="ProtocolType.Tcp"/>-based <see cref="Socket"/>s.
/// </summary>
/// <param name="hostService">The service used for resolving host names.</param>
internal sealed class JavaSocketFactory(HostService hostService)
{
    /// <summary>
    /// Creates a <see cref="Socket"/> to the given address and port.
    /// </summary>
    /// <param name="address">The address to create the <see cref="Socket"/> to.</param>
    /// <param name="port">The port to create the <see cref="Socket"/> to.</param>
    /// <param name="token">A <see cref="CancellationToken"/> that can be used to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="Result"/> containing the created <see cref="Socket"/>.</returns>
    public async Task<Result<Socket>> ConnectAsync(string address, ushort port, CancellationToken token)
    {
        var resolving = await hostService.ResolveHostAsync(address, token);

        if (!resolving.IsSuccess(out var host))
        {
            return resolving.AsFailure<Socket>();
        }

        var socket = new Socket(host.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            await socket.ConnectAsync(host, port, token);
        }
        catch (SocketException)
        {
            return Result.Failure<Socket>("Could not connect to the server.");
        }

        return socket;
    }
}