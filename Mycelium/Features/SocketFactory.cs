using System.Net.Sockets;
using LightResults;

namespace Mycelium.Features;

/// <summary>
/// A factory for creating connected <see cref="Socket"/> instances.
/// </summary>
internal sealed class SocketFactory
{
    /// <summary>
    /// Creates and attempts to connect a <see cref="Socket"/>.
    /// </summary>
    /// <param name="address">The <see cref="Address"/> to connect to.</param>
    /// <param name="type">The <see cref="Socket"/>'s type.</param>
    /// <param name="protocol">The <see cref="Socket"/>'s protocol</param>
    /// <param name="token">A <see cref="CancellationToken"/> that can be used to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="Result"/> containing the <see cref="Socket"/>.</returns>
    public async Task<Result<Socket>> ConnectAsync(Address address, SocketType type, ProtocolType protocol, CancellationToken token)
    {
        var socket = new Socket(type, protocol);

        try
        {
            await socket.ConnectAsync(address.First, address.Port, token);
        }
        catch (SocketException)
        {
            return Result.Failure<Socket>("Could not establish a connection.");
        }

        return socket;
    }
}