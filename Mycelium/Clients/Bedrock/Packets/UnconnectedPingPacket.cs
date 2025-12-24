using System.Collections.Immutable;
using System.Net.Sockets;

namespace Mycelium.Clients.Bedrock.Packets;

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

    public static async Task<bool> TryWriteAsync(Socket socket)
    {
        return await socket.SendAsync(Packet.AsMemory()) == Packet.Length;
    }
}