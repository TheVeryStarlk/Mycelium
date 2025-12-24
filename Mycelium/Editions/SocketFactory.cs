using System.Net.Sockets;

namespace Mycelium.Editions;

internal sealed class SocketFactory : ISocketFactory
{
    public async Task<Socket> ConnectAsync(Address address, SocketType type, ProtocolType protocol, CancellationToken token)
    {
        var socket = new Socket(type, protocol);

        try
        {
            await socket.ConnectAsync(address.Host, address.Port, token);
        }
        catch (Exception exception) when (exception is SocketException or OperationCanceledException)
        {
            socket.Dispose();
        }

        return socket;
    }
}