using System.Buffers.Binary;
using System.IO.Pipelines;

namespace Mycelium.Java.Packets;

internal static class StatusRequestPacket
{
    public static async ValueTask WriteAsync(PipeWriter output, string address, ushort port, CancellationToken token)
    {
        // Handshake packet only.
        var length = VariableLength.GetByteCount(address) + sizeof(ushort) + 7;

        // Accounts for the status request packet as well.
        var total = length + VariableLength.GetByteCount(length) + 2;

        var span = output.GetSpan(total);
        var index = 0;

        index += VariableLength.Write(span, length);

        span[index++] = 0;

        // Version.
        span[index++] = byte.MaxValue;
        span[index++] = byte.MaxValue;
        span[index++] = byte.MaxValue;
        span[index++] = byte.MaxValue;
        span[index++] = 15;

        index += VariableLength.Write(span[index..], address);

        BinaryPrimitives.WriteUInt16BigEndian(span[index..(index += sizeof(ushort))], port);

        span[index++] = 1;

        // Status request packet.
        span[index++] = 1;
        span[index++] = 0;

        output.Advance(index);

        await output.FlushAsync(token);
    }
}