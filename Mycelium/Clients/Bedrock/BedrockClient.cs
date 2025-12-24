using System.Net.Sockets;
using Mycelium.Clients.Bedrock.Packets;

namespace Mycelium.Clients.Bedrock;

public sealed class BedrockClient(ISocketFactory factory)
{
    public BedrockClient() : this(new SocketFactory())
    {
        // What about logging?
    }

    public async Task<BedrockResponse?> RequestStatusAsync(string address, CancellationToken token = default)
    {
        if (!Address.TryParse(address, out var result))
        {
            throw new MyceliumException("Invalid address.");
        }

        var status = await RequestStatusAsync(result, token);

        return !BedrockResponse.TryCreate(status, out var response) 
            ? throw new MyceliumException("Received invalid status.") 
            : response;
    }
    
    public async Task<string?> RequestStatusAsync(Address address, CancellationToken token = default)
    {
        using var socket = await factory.ConnectAsync(address, SocketType.Dgram, ProtocolType.Udp, token);

        if (!await UnconnectedPingPacket.TryWriteAsync(socket))
        {
            throw new MyceliumException("Failed to ping the server.");
        }

        return await UnconnectedPongPacket.ReadAsync(socket, token);
    }
}