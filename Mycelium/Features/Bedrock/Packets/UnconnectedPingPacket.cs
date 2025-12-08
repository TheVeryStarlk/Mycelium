using System.Collections.Immutable;
using System.Net.Sockets;
using LightResults;

namespace Mycelium.Features.Bedrock.Packets;

/// <summary>
/// Represents a Minecraft Bedrock unconnected ping packet.
/// </summary>
internal static class UnconnectedPingPacket
{
    private static readonly ImmutableArray<byte> Packet =
    [
        // Identifier
        1,

        // Time.
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,

        // Magic.
        0,
        255,
        255,
        0,
        254,
        254,
        254,
        254,
        253,
        253,
        253,
        253,
        18,
        52,
        86,
        120,

        // GUID.
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0
    ];

    /// <summary>
    /// Writes an unconnected ping packet.
    /// </summary>
    /// <param name="socket">The <see cref="Socket"/> to write to.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous write operation.</returns>
    public static async Task<Result> WriteAsync(Socket socket)
    {
        var sent = await socket.SendAsync(Packet.AsMemory());
        return sent == Packet.Length ? Result.Success() : Result.Failure("Failed to send the packet.");
    }
}