using System.IO.Pipelines;

namespace Mycelium.Java.Packets;

internal static class StatusRequestPacket
{
    public static async ValueTask WriteAsync(PipeWriter output, string address, ushort port)
    {
        // Handshake packet only.
        var length = Variable.GetByteCount(0) + Variable.GetByteCount(-1) + Variable.GetByteCount(address) + sizeof(ushort) + Variable.GetByteCount(1);

        // Accounts for the status request packet as well.
        var total = length + Variable.GetByteCount(length) + Variable.GetByteCount(1) + Variable.GetByteCount(0);

        var span = output.GetSpan();

        SpanWriter
            .Create(span)
            .WriteVariableInteger(length)
            .WriteVariableInteger(0)
            .WriteVariableInteger(-1)
            .WriteVariableString(address)
            .WriteUnsignedShort(port)
            .WriteVariableInteger(1)
            .WriteVariableInteger(1)
            .WriteVariableInteger(0);

        output.Advance(total);
        await output.FlushAsync();
    }
}