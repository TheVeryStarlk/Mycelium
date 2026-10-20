using System.Buffers.Binary;
using System.IO.Pipelines;
using System.Text;

namespace Mycelium.Java.Packets;

internal static class LegacyStatusRequestPacket
{
    private static ReadOnlySpan<byte> Channel =>
    [
        250,
        0,
        11,
        0,
        77,
        0,
        67,
        0,
        124,
        0,
        80,
        0,
        105,
        0,
        110,
        0,
        103,
        0,
        72,
        0,
        111,
        0,
        115,
        0,
        116
    ];

    public static async ValueTask WriteHostAsync(PipeWriter output, string address, ushort port, CancellationToken token)
    {
        const byte version = 78;

        // Version, address and port.
        var length = sizeof(byte) + sizeof(ushort) + address.Length * sizeof(char) + sizeof(int);

        var span = output.GetSpan(2 + Channel.Length + sizeof(ushort) + length);
        var index = 0;

        // Server list ping.
        span[index++] = 254;
        span[index++] = 1;

        Channel.CopyTo(span[index..]);
        index += Channel.Length;

        BinaryPrimitives.WriteUInt16BigEndian(span[index..(index += sizeof(ushort))], (ushort) length);

        span[index++] = version;

        BinaryPrimitives.WriteUInt16BigEndian(span[index..(index += sizeof(ushort))], (ushort) address.Length);
        index += Encoding.BigEndianUnicode.GetBytes(address, span[index..]);

        BinaryPrimitives.WriteInt32BigEndian(span[index..(index += sizeof(int))], port);

        output.Advance(index);

        await output.FlushAsync(token);
    }

    public static async ValueTask WriteExtendedAsync(PipeWriter output, CancellationToken token)
    {
        var span = output.GetSpan(2);

        // Server list ping.
        span[0] = 254;
        span[1] = 1;

        output.Advance(2);

        await output.FlushAsync(token);
    }

    public static async ValueTask WriteBetaAsync(PipeWriter output, CancellationToken token)
    {
        var span = output.GetSpan(1);

        // Server list ping.
        span[0] = 254;

        output.Advance(1);

        await output.FlushAsync(token);
    }
}
