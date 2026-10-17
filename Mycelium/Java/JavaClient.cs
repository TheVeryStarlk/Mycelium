using System.IO.Pipelines;
using System.Net.Sockets;
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
    /// <param name="version">The <see cref="JavaVersion"/> to request the status with.</param>
    /// <param name="factory">The <see cref="ISocketFactory"/> for the client.</param>
    /// <param name="logger">The <see cref="ILogger{T}"/> for the client.</param>
    /// <param name="token">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> that represents the asynchronous status request, yielding a <see cref="JavaResponse"/>.
    /// </returns>
    /// <exception cref="MyceliumException">Invalid address.</exception>
    /// <exception cref="MyceliumException">Failed to read status.</exception>
    public static async ValueTask<JavaResponse?> RequestStatusAsync(
        string address,
        JavaVersion version,
        ILogger<JavaClient>? logger = null,
        ISocketFactory? factory = null,
        CancellationToken token = default)
    {
        if (!Address.TryParse(address, out var result))
        {
            throw new MyceliumException("Invalid address.");
        }

        var client = new JavaClient(logger ?? NullLogger<JavaClient>.Instance, factory ?? new SocketFactory());
        var status = await client.RequestStatusAsync(result, version, token);

        return JavaResponse.TryCreate(status, version, out var response)
            ? response
            : throw new MyceliumException("Failed to read status.");
    }

    /// <summary>
    /// Requests the raw status of a Minecraft server.
    /// </summary>
    /// <param name="address">The <see cref="Address"/> of the server.</param>
    /// <param name="version">The <see cref="JavaVersion"/> to request the status with.</param>
    /// <param name="token">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> that represents the asynchronous status request,
    /// yielding the raw status to be converted with <see cref="JavaResponse.TryCreate"/> using the same <paramref name="version"/>.
    /// </returns>
    /// <exception cref="MyceliumException">Received incomplete packet.</exception>
    /// <exception cref="MyceliumException">Failed to read packet.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="version"/> is not a defined <see cref="JavaVersion"/>.</exception>
    public async ValueTask<string> RequestStatusAsync(Address address, JavaVersion version, CancellationToken token = default)
    {
        using var socket = await factory.ConnectAsync(address, SocketType.Stream, ProtocolType.Tcp, token);

        await using var stream = new NetworkStream(socket, true);

        var output = PipeWriter.Create(stream);
        var input = PipeReader.Create(stream);

        try
        {
            switch (version)
            {
                case JavaVersion.Modern:
                    await StatusRequestPacket.WriteAsync(output, address.Host, address.Port, token);
                    return await StatusResponsePacket.ReadAsync(input, token);

                case JavaVersion.LegacyHost:
                    await LegacyStatusRequestPacket.WriteHostAsync(output, address.Host, address.Port, token);
                    break;

                case JavaVersion.LegacyExtended:
                    await LegacyStatusRequestPacket.WriteExtendedAsync(output, token);
                    break;

                case JavaVersion.LegacyBeta:
                    await LegacyStatusRequestPacket.WriteBetaAsync(output, token);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(version), version, null);
            }

            logger.LogTrace("Successfully wrote {Version} status request packet", version);

            return await LegacyStatusResponsePacket.ReadAsync(input, token);
        }
        finally
        {
            await output.CompleteAsync();
            await input.CompleteAsync();
        }
    }
}