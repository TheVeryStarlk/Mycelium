using System.Buffers.Binary;
using System.Text;

namespace Mycelium.Java.Packets;

internal ref struct SpanWriter
{
    private int position;

    private readonly Span<byte> span;

    private SpanWriter(Span<byte> span)
    {
        this.span = span;
    }

    public static SpanWriter Create(Span<byte> span)
    {
        return new SpanWriter(span);
    }

    public SpanWriter WriteUnsignedShort(ushort value)
    {
        BinaryPrimitives.WriteUInt16BigEndian(span[position..(position += sizeof(ushort))], value);
        return this;
    }

    public SpanWriter WriteVariableInteger(int value)
    {
        var unsigned = (uint) value;

        do
        {
            var current = (byte) (unsigned & 127);
            unsigned >>= 7;

            if (unsigned != 0)
            {
                current |= 128;
            }

            span[position++] = current;
        } while (unsigned != 0);

        return this;
    }

    public SpanWriter WriteVariableString(string value)
    {
        WriteVariableInteger(Encoding.UTF8.GetByteCount(value));
        position += Encoding.UTF8.GetBytes(value, span[position..]);

        return this;
    }
}