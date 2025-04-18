using System.Numerics;
using System.Text;

namespace Mycelium.Features.Java.Packets;

/// <summary>
/// Provides methods for reading/writing variable types.
/// </summary>
internal static class Variable
{
    /// <summary>
    /// Calculates the number of <see cref="byte"/>s produced by encoding a variable-<see cref="int"/>.
    /// </summary>
    /// <param name="value">The variable-<see cref="int"/>.</param>
    /// <returns>The number of <see cref="byte"/>s produced by encoding the variable-<see cref="int"/>.</returns>
    public static int GetByteCount(int value)
    {
        return (BitOperations.LeadingZeroCount((uint) value | 1) - 38) * -1171 >> 13;
    }

    /// <summary>
    /// Calculates the number of <see cref="byte"/>s produced by encoding a variable-<see cref="int"/> prefixed <see cref="string"/>.
    /// </summary>
    /// <param name="value">The variable-<see cref="int"/> prefixed <see cref="string"/>.</param>
    /// <returns>The number of <see cref="byte"/>s produced by encoding the variable-<see cref="int"/> prefixed <see cref="string"/>.</returns>
    public static int GetByteCount(string value)
    {
        var length = Encoding.UTF8.GetByteCount(value);
        return GetByteCount(length) + length;
    }

    /// <summary>
    /// Writes a variable-<see cref="int"/>.
    /// </summary>
    /// <param name="span">The <see cref="Span{T}"/> to write to.</param>
    /// <param name="value">The variable-<see cref="int"/> to write.</param>
    /// <returns>The amount of written <see cref="byte"/>s.</returns>
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

    /// <summary>
    /// Writes a variable-<see cref="int"/> prefixed <see cref="string"/>.
    /// </summary>
    /// <param name="span">The <see cref="Span{T}"/> to write to.</param>
    /// <param name="value">The variable-<see cref="int"/> prefixed <see cref="string"/> to write.</param>
    /// <returns>The amount of written <see cref="byte"/>s.</returns>
    public static int Write(Span<byte> span, string value)
    {
        var index = 0;

        index += Write(span, Encoding.UTF8.GetByteCount(value));
        index += Encoding.UTF8.GetBytes(value, span[index..]);

        return index;
    }
}