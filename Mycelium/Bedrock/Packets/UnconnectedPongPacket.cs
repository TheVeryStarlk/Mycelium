using System.Buffers;
using System.Net.Sockets;
using System.Text;

namespace Mycelium.Bedrock.Packets;

internal static class UnconnectedPongPacket
{
    public static async ValueTask<string?> ReadAsync(Socket socket, CancellationToken token)
    {
        // https://github.com/CloudburstMC/Network/blob/0eb940aa5b60f0c156dda02f576a3f194d25e7ae/transport-raknet/src/main/java/org/cloudburstmc/netty/channel/raknet/RakConstants.java#L30.
        using var owner = MemoryPool<byte>.Shared.Rent(1400);

        var received = await socket.ReceiveAsync(owner.Memory, token);

        // Identifier, two longs, the magic and the string's unsigned short prefix.
        // Probably should validate the payload too.
        const byte skip = 35;

        return received > skip ? Encoding.UTF8.GetString(owner.Memory[skip..received].Span) : null;
    }
}