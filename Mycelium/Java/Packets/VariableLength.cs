using System.Numerics;
using System.Text;

namespace Mycelium.Java.Packets;

internal static class VariableLength
{
    public static int GetByteCount(int value)
    {
        return (BitOperations.LeadingZeroCount((uint) value | 1) - 38) * -1171 >> 13;
    }

    public static int GetByteCount(string value)
    {
        var length = Encoding.UTF8.GetByteCount(value);
        return GetByteCount(length) + length;
    }

    public static int Write(Span<byte> span, int value)
    {
        var index = 0;
        var unsigned = (uint) value;

        do
        {
            var current = (byte) (unsigned & 127);
            unsigned >>= 7;

            if (unsigned != 0)
            {
                current |= 128;
            }

            span[index++] = current;
        } while (unsigned != 0);

        return index;
    }

    public static int Write(Span<byte> span, string value)
    {
        var index = Write(span, Encoding.UTF8.GetByteCount(value));
        return index + Encoding.UTF8.GetBytes(value, span[index..]);
    }
}