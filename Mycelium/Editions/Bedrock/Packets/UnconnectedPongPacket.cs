using System.Buffers;
using System.Net.Sockets;
using System.Text;

namespace Mycelium.Editions.Bedrock.Packets;

internal static class UnconnectedPongPacket
{
    public static async Task<string?> ReadAsync(Socket socket, CancellationToken token)
    {
        // Maximum transmission unit.
        var buffer = ArrayPool<byte>.Shared.Rent(1500);

        try
        {
            var received = await socket.ReceiveAsync(buffer, token);

            // Identifier, two longs, the magic and the string's unsigned short prefix.
            // Probably should validate the payload too.
            const byte skip = 35;

            var slice = buffer.AsSpan()[skip..received];

            return received > skip ? Encoding.UTF8.GetString(slice) : null;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}