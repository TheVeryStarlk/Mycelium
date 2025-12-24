using System.Net.Sockets;

namespace Mycelium.Editions;

public interface ISocketFactory
{
    public Task<Socket> ConnectAsync(Address address, SocketType type, ProtocolType protocol, CancellationToken token);
}