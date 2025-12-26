using System.IO.Pipelines;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Mycelium.Java.Packets;

namespace Mycelium.Java;

/// <summary>
/// Represents a simple Minecraft Java edition client.
/// </summary>
public sealed class JavaClient
{
    private readonly ILogger<JavaClient> logger = NullLogger<JavaClient>.Instance;
    private readonly ISocketFactory factory = new SocketFactory();

    /// <summary>
    /// Creates a simple Minecraft client with a <see cref="NullLogger"/> and a default <see cref="ISocketFactory"/> instance.
    /// </summary>
    public JavaClient()
    {
        // ...
    }

    /// <summary>
    /// Creates a simple Minecraft client.
    /// </summary>
    public JavaClient(ILogger<JavaClient> logger, ISocketFactory factory)
    {
        this.logger = logger;
        this.factory = factory;
    }

    /// <summary>
    /// Creates a simple Minecraft client with a <see cref="NullLogger"/>.
    /// </summary>
    public JavaClient(ILogger<JavaClient> logger)
    {
        this.logger = logger;
    }

    /// <summary>
    /// Creates a simple Minecraft client with a default <see cref="ISocketFactory"/> instance.
    /// </summary>
    public JavaClient(ISocketFactory factory)
    {
        this.factory = factory;
    }

    /// <summary>
    /// Attempts to request the status of a Minecraft server.
    /// </summary>
    /// <param name="address">The <see cref="string"/> of the server in <see cref="Address"/> format.</param>
    /// <param name="token">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> that represents the asynchronous status request, yielding a <see cref="JavaResponse"/>.
    /// </returns>
    /// <exception cref="MyceliumException">Invalid address.</exception>
    /// <exception cref="MyceliumException">Failed to read status.</exception>
    public async ValueTask<JavaResponse?> RequestStatusAsync(string address, CancellationToken token = default)
    {
        if (!Address.TryParse(address, out var result))
        {
            throw new MyceliumException("Invalid address.");
        }

        var status = await RequestStatusAsync(result, token);

        return JavaResponse.TryCreate(status, out var response)
            ? response
            : throw new MyceliumException("Failed to read status.");
    }

    public async ValueTask<string> RequestStatusAsync(Address address, CancellationToken token = default)
    {
        var socket = await factory.ConnectAsync(address, SocketType.Stream, ProtocolType.Tcp, token);

        await using var stream = new NetworkStream(socket, true);

        var output = PipeWriter.Create(stream);
        var input = PipeReader.Create(stream);

        try
        {
            await StatusRequestPacket.WriteAsync(output, address.Host, address.Port, token);
            return Encoding.UTF8.GetString(await StatusResponsePacket.ReadAsync(input, token));
        }
        finally
        {
            await output.CompleteAsync();
            await input.CompleteAsync();
        }
    }
}