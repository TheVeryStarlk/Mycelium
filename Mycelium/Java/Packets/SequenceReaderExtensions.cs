using System.Buffers;

namespace Mycelium.Java.Packets;

internal static class SequenceReaderExtensions
{
    extension(ref SequenceReader<byte> reader)
    {
        public bool TryReadVariableInteger(out int value)
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

        public bool TryReadVariableString(out ReadOnlySequence<byte> value)
        {
            value = default;
            return reader.TryReadVariableInteger(out var length) && length >= 0 && reader.TryReadExact(length, out value);
        }

        public bool TryReadBigEndianString(out ReadOnlySequence<byte> value)
        {
            value = default;

            // The length is in UTF-16 characters, not bytes.
            return reader.TryReadBigEndian(out short length) && length >= 0 && reader.TryReadExact(length * sizeof(char), out value);
        }
    }
}