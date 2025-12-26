using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Mycelium.Bedrock.Packets;

namespace Mycelium.Bedrock;

/// <summary>
/// Represents a simple Minecraft Bedrock edition client.
/// </summary>
public sealed class BedrockClient
{
    private readonly ILogger<BedrockClient> logger = NullLogger<BedrockClient>.Instance;
    private readonly ISocketFactory factory = new SocketFactory();

    /// <summary>
    /// Creates a simple Minecraft client with a <see cref="NullLogger"/> and a default <see cref="ISocketFactory"/> instance.
    /// </summary>
    public BedrockClient()
    {
        // ...
    }

    /// <summary>
    /// Creates a simple Minecraft client.
    /// </summary>
    public BedrockClient(ILogger<BedrockClient> logger, ISocketFactory factory)
    {
        this.logger = logger;
        this.factory = factory;
    }

    /// <summary>
    /// Creates a simple Minecraft client with a <see cref="NullLogger"/>.
    /// </summary>
    public BedrockClient(ILogger<BedrockClient> logger)
    {
        this.logger = logger;
    }

    /// <summary>
    /// Creates a simple Minecraft client with a default <see cref="ISocketFactory"/> instance.
    /// </summary>
    public BedrockClient(ISocketFactory factory)
    {
        this.factory = factory;
    }

    /// <summary>
    /// Attempts to request the status of a Minecraft server.
    /// </summary>
    /// <param name="address">The <see cref="string"/> of the server in <see cref="Address"/> format.</param>
    /// <param name="factory">The <see cref="ISocketFactory"/> for the client.</param>
    /// <param name="logger">The <see cref="ILogger{T}"/> for the client.</param>
    /// <param name="token">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> that represents the asynchronous status request, yielding a <see cref="BedrockResponse"/>.
    /// </returns>
    /// <exception cref="MyceliumException">Invalid address.</exception>
    /// <exception cref="MyceliumException">Failed to read status.</exception>
    /// <exception cref="MyceliumException">Failed to ping the server.</exception>
    public static async ValueTask<BedrockResponse?> RequestStatusAsync(
        string address, 
        ILogger<BedrockClient>? logger = null,
        ISocketFactory? factory = null,
        CancellationToken token = default)
    {
        if (!Address.TryParse(address, out var result))
        {
            throw new MyceliumException("Invalid address.");
        }
        
        var client = new BedrockClient(logger ?? NullLogger<BedrockClient>.Instance, factory ?? new SocketFactory());
        var status = await client.RequestStatusAsync(result, token);

        return BedrockResponse.TryCreate(status, out var response)
            ? response
            : throw new MyceliumException("Failed to read status.");
    }

    /// <summary>
    /// Attempts to request the status of a Minecraft server.
    /// </summary>
    /// <param name="address">The <see cref="Address"/> of the server.</param>
    /// <param name="token">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> that represents the asynchronous status request, yielding a <see cref="BedrockResponse"/>.
    /// </returns>
    /// <exception cref="MyceliumException">Invalid address.</exception>
    /// <exception cref="MyceliumException">Failed to read status.</exception>
    /// <exception cref="MyceliumException">Failed to ping the server.</exception>
    public async ValueTask<string?> RequestStatusAsync(Address address, CancellationToken token = default)
    {
        using var socket = await factory.ConnectAsync(address, SocketType.Dgram, ProtocolType.Udp, token);

        if (await UnconnectedPingPacket.TryWriteAsync(socket))
        {
            return await UnconnectedPongPacket.ReadAsync(socket, token);
        }
        
        logger.LogTrace("Unconnected ping packet was invalid, bad headers?");

        throw new MyceliumException("Failed to ping the server.");
    }
}