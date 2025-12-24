using System.Net.Sockets;
using Microsoft.Extensions.Options;
using Mycelium.Editions.Bedrock.Packets;
using Mycelium.Hosting;

namespace Mycelium.Editions.Bedrock;

public sealed class BedrockClient
{
    private readonly IOptions<MyceliumOptions> options = Options.Create(new MyceliumOptions());
    private readonly ISocketFactory factory = new SocketFactory();

    public BedrockClient()
    {
        // Nothing...
    }
    
    public BedrockClient(IOptions<MyceliumOptions> options)
    {
        this.options = options;
    }
    
    public BedrockClient(ISocketFactory factory)
    {
        this.factory = factory;
    }
    
    public BedrockClient(IOptions<MyceliumOptions> options, ISocketFactory factory)
    {
        this.options = options;
        this.factory = factory;
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