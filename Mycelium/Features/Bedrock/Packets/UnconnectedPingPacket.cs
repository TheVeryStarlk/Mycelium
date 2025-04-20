using System.Net.Sockets;

namespace Mycelium.Features.Bedrock.Packets;

/// <summary>
/// Represents a Minecraft Bedrock unconnected pong packet.
/// </summary>
internal static class UnconnectedPingPacket
{
    /// <summary>
    /// Writes an unconnected pong packet.
    /// </summary>
    /// <param name="socket">The <see cref="Socket"/> to write to.</param>
    public static void Write(Socket socket)
    {
        ReadOnlySpan<byte> packet =
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

        // Worth it to use the asynchronous method?
        socket.Send(packet);
    }
}