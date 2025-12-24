using System.Net.Sockets;
using Mycelium.Editions.Bedrock.Packets;

namespace Mycelium.Editions.Bedrock;

public sealed class BedrockClient(ISocketFactory factory)
{
    public BedrockClient() : this(new SocketFactory())
    {
        // What about logging?
    }

    public async Task<string?> RequestStatusAsync(Address address, CancellationToken token)
    {
        using var socket = await factory.ConnectAsync(address, SocketType.Dgram, ProtocolType.Udp, token);

        if (!await UnconnectedPingPacket.TryWriteAsync(socket))
        {
            throw new MyceliumException("Failed to ping the server.");
        }

        return await UnconnectedPongPacket.ReadAsync(socket, token);
    }
}