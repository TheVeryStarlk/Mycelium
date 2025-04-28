using System.Buffers;

namespace Mycelium.Features.Java.Packets;

/// <summary>
/// Provides extension methods to <see cref="SequenceReader{T}"/>.
/// </summary>
internal static class SequenceReaderExtensions
{
    /// <summary>
    /// Tries to read a variable-<see cref="int"/> from a <see cref="SequenceReader{T}"/>.
    /// </summary>
    /// <param name="reader">The <see cref="SequenceReader{T}"/> to read from.</param>
    /// <param name="value">The read <see cref="int"/> value.</param>
    /// <returns>True if the variable-<see cref="int"/> was converted successfully, otherwise, false.</returns>
    public static bool TryReadVariableInteger(ref this SequenceReader<byte> reader, out int value)
    {
        value = 0;

        var index = 0;
        var result = 0;

        byte current;

        do
        {
            if (!reader.TryRead(out current))
            {
                return false;
            }

            result |= (current & 127) << 7 * index;

            index++;

            if (index > 5)
            {
                return false;
            }
        } while ((current & 128) != 0);

        value = result;

        return true;
    }

    /// <summary>
    /// Tries to read a variable-<see cref="int"/> prefixed <see cref="string"/> from a <see cref="SequenceReader{T}"/>.
    /// </summary>
    /// <param name="reader">The <see cref="SequenceReader{T}"/> to read from.</param>
    /// <param name="value">The read <see cref="string"/> value.</param>
    /// <returns>True if the <see cref="string"/> was converted successfully, otherwise, false.</returns>
    public static bool TryReadVariableString(ref this SequenceReader<byte> reader, out ReadOnlySequence<byte> value)
    {
        value = default;
        return reader.TryReadVariableInteger(out var length) && reader.TryReadExact(length, out value);
    }
}