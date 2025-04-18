using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Mycelium.Java.Packets;

internal static class SequenceReaderExtensions
{
    public static bool TryReadVariableInteger(ref this SequenceReader<byte> reader, out int value)
    {
        var numbersRead = 0;
        var result = 0;

        byte read;

        do
        {
            if (!reader.TryRead(out read))
            {
                value = 0;

                return false;
            }

            var temporaryValue = read & 0b01111111;
            result |= temporaryValue << 7 * numbersRead;

            numbersRead++;

            if (numbersRead <= 5)
            {
                continue;
            }

            value = 0;

            return false;
        } while ((read & 0b10000000) != 0);

        value = result;

        return true;
    }

    public static bool TryReadVariableString(ref this SequenceReader<byte> reader, [NotNullWhen(true)] out string? value)
    {
        value = null;

        if (!reader.TryReadVariableInteger(out var length) || !reader.TryReadExact(length, out var buffer))
        {
            return false;
        }

        value = Encoding.UTF8.GetString(buffer);

        return true;
    }
}