using System.Buffers.Binary;
using System.IO.Pipelines;

namespace Mycelium.Java.Packets;

/// <summary>
/// Represents a Minecraft status request packet.
/// </summary>
internal static class StatusRequestPacket
{
    /// <summary>
    /// Writes a handshake and a status request packet.
    /// </summary>
    /// <param name="output">The <see cref="PipeWriter"/> to write to.</param>
    /// <param name="address">The address used in the handshake packet.</param>
    /// <param name="port">The port used in the handshake packet.</param>
    public static async ValueTask WriteAsync(PipeWriter output, string address, ushort port)
    {
        // Handshake packet only.
        var length = Variable.GetByteCount(address) + sizeof(ushort) + 7;

        // Accounts for the status request packet as well.
        var total = length + Variable.GetByteCount(length) + 2;

        var span = output.GetSpan(total);
        var index = 0;

        index += Variable.Write(span, length);

        span[index++] = 0;

        Span<byte> version =
        [
            byte.MaxValue,
            byte.MaxValue,
            byte.MaxValue,
            byte.MaxValue,
            15
        ];

        version.CopyTo(span[index..(index += 5)]);

        index += Variable.Write(span[index..], address);

        BinaryPrimitives.WriteUInt16BigEndian(span[index..(index += sizeof(ushort))], port);

        span[index++] = 1;
        span[index++] = 1;
        span[index++] = 0;

        output.Advance(index);
        await output.FlushAsync();
    }
}