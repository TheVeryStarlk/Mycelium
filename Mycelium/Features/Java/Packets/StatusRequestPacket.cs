using System.Buffers.Binary;
using System.IO.Pipelines;

namespace Mycelium.Features.Java.Packets;

/// <summary>
/// Represents a Minecraft Java status request packet.
/// </summary>
internal static class StatusRequestPacket
{
    /// <summary>
    /// Writes a handshake and a status request packet.
    /// </summary>
    /// <param name="output">The <see cref="PipeWriter"/> to write to.</param>
    /// <param name="address">The address used in the handshake packet.</param>
    /// <param name="port">The port used in the handshake packet.</param>
    /// <param name="token">A <see cref="CancellationToken"/> that can be used to cancel the asynchronous operation.</param>
    public static async ValueTask WriteAsync(PipeWriter output, string address, ushort port, CancellationToken token)
    {
        // Handshake packet only.
        var length = Variable.GetByteCount(address) + sizeof(ushort) + 7;

        // Accounts for the status request packet as well.
        var total = length + Variable.GetByteCount(length) + 2;

        var span = output.GetSpan(total);
        var index = 0;

        index += Variable.Write(span, length);

        span[index++] = 0;

        // Version.
        span[index++] = byte.MaxValue;
        span[index++] = byte.MaxValue;
        span[index++] = byte.MaxValue;
        span[index++] = byte.MaxValue;
        span[index++] = 15;

        index += Variable.Write(span[index..], address);

        BinaryPrimitives.WriteUInt16BigEndian(span[index..(index += sizeof(ushort))], port);

        span[index++] = 1;

        // Status request packet.
        span[index++] = 1;
        span[index++] = 0;

        output.Advance(index);
        await output.FlushAsync(token);
    }
}