using System.Net.Sockets;

namespace Mycelium;

/// <summary>
/// A factory abstraction for creating connections to an endpoint.
/// </summary>
public interface ISocketFactory
{
    /// <summary>
    /// Creates a new connection to an endpoint.
    /// </summary>
    /// <param name="address">The <see cref="Address"/> to connect to.</param>
    /// <param name="type">The <see cref="Socket"/>'s <see cref="SocketType"/>.</param>
    /// <param name="protocol">The <see cref="Socket"/>'s <see cref="ProtocolType"/>.</param>
    /// <param name="token">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> that represents the asynchronous connect, yielding the <see cref="Socket"/> for the new connection when completed.
    /// </returns>
    public ValueTask<Socket> ConnectAsync(Address address, SocketType type, ProtocolType protocol, CancellationToken token = default);
}

internal sealed class SocketFactory : ISocketFactory
{
    public async ValueTask<Socket> ConnectAsync(Address address, SocketType type, ProtocolType protocol, CancellationToken token)
    {
        var socket = new Socket(type, protocol);

        await socket.ConnectAsync(address.Host, address.Port, token);

        return socket;
    }
}