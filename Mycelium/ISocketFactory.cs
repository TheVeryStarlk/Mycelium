using System.Net.Sockets;

namespace Mycelium;

public interface ISocketFactory
{
    public Task<Socket> ConnectAsync(Address address, SocketType type, ProtocolType protocol, CancellationToken token);
}

internal sealed class SocketFactory : ISocketFactory
{
    public async Task<Socket> ConnectAsync(Address address, SocketType type, ProtocolType protocol, CancellationToken token)
    {
        var socket = new Socket(type, protocol);

        await socket.ConnectAsync(address.Host, address.Port, token);

        return socket;
    }
}