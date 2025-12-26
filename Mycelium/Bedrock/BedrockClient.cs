using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Mycelium.Bedrock.Packets;

namespace Mycelium.Bedrock;

public sealed class BedrockClient
{
    private readonly ILogger<BedrockClient> logger = NullLogger<BedrockClient>.Instance;
    private readonly ISocketFactory factory = new SocketFactory();

    public BedrockClient()
    {
        // ...
    }

    public BedrockClient(ILogger<BedrockClient> logger, ISocketFactory factory)
    {
        this.logger = logger;
        this.factory = factory;
    }
    
    public BedrockClient(ILogger<BedrockClient> logger)
    {
        this.logger = logger;
    }
    
    public BedrockClient(ISocketFactory factory)
    {
        this.factory = factory;
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
            : throw new MyceliumException("Failed to read status.");
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