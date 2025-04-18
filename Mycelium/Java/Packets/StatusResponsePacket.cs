using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipelines;

namespace Mycelium.Java.Packets;

internal static class StatusResponsePacket
{
    public static async ValueTask<string?> ReadAsync(PipeReader input)
    {
        while (true)
        {
            var result = await input.ReadAsync();

            var buffer = result.Buffer;

            var consumed = buffer.Start;
            var examined = buffer.End;

            try
            {
                if (TryRead(ref buffer, out var status))
                {
                    consumed = buffer.Start;
                    examined = consumed;

                    return status;
                }

                if (result.IsCompleted)
                {
                    if (buffer.Length > 0)
                    {
                        // The message is incomplete and there's no more data to process.
                        throw new InvalidDataException("Incomplete message.");
                    }

                    break;
                }
            }
            finally
            {
                input.AdvanceTo(consumed, examined);
            }
        }

        return null;

    }

    private static bool TryRead(ref ReadOnlySequence<byte> sequence, [NotNullWhen(true)] out string? response)
    {
        var reader = new SequenceReader<byte>(sequence);

        response = null;

        if (!reader.TryReadVariableInteger(out _)
            || !reader.TryReadVariableInteger(out var identifier)
            || !reader.TryReadVariableString(out response)
            || identifier != 0)
        {
            return false;
        }

        sequence = sequence.Slice(reader.Position);

        return true;
    }
}