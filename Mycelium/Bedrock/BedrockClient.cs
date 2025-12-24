using System.Net.Sockets;
using Mycelium.Bedrock.Packets;

namespace Mycelium.Bedrock;

public sealed class BedrockClient(ISocketFactory factory)
{
    public BedrockClient() : this(new SocketFactory())
    {
        // What about logging?
    }

    public async ValueTask<BedrockResponse?> RequestStatusAsync(string address, CancellationToken token = default)
    {
        if (!Address.TryParse(address, out var result))
        {
            throw new MyceliumException("Invalid address.");
        }

        var status = await RequestStatusAsync(result, token);

        return BedrockResponse.TryCreate(status, out var response) 
            ? response 
            : throw new MyceliumException("Received invalid status.");
    }
    
    public async ValueTask<string?> RequestStatusAsync(Address address, CancellationToken token = default)
    {
        using var socket = await factory.ConnectAsync(address, SocketType.Dgram, ProtocolType.Udp, token);

        if (!await UnconnectedPingPacket.TryWriteAsync(socket))
        {
            throw new MyceliumException("Failed to ping the server.");
        }

        return await UnconnectedPongPacket.ReadAsync(socket, token);
    }
}