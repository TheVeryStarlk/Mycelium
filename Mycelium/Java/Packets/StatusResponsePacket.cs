using System.Buffers;
using System.IO.Pipelines;

namespace Mycelium.Java.Packets;

internal static class StatusResponsePacket
{
    public static async ValueTask<ReadOnlySequence<byte>> ReadAsync(PipeReader input, CancellationToken token)
    {
        while (true)
        {
            var result = await input.ReadAsync(token);
            var buffer = result.Buffer;

            var consumed = buffer.Start;
            var examined = buffer.End;

            try
            {
                if (TryRead(ref buffer, out var status))
                {
                    return status;
                }

                if (result.IsCompleted)
                {
                    if (buffer.Length > 0)
                    {
                        throw new MyceliumException("Received incomplete packet.");
                    }

                    break;
                }
            }
            finally
            {
                input.AdvanceTo(consumed, examined);
            }
        }

        return ReadOnlySequence<byte>.Empty;
    }

    private static bool TryRead(ref ReadOnlySequence<byte> sequence, out ReadOnlySequence<byte> status)
    {
        var reader = new SequenceReader<byte>(sequence);

        status = default;

        if (!reader.TryReadVariableInteger(out _) || !reader.TryReadVariableInteger(out var identifier)
                                                  || !reader.TryReadVariableString(out status))
        {
            return false;
        }

        if (identifier != 0)
        {
            return false;
        }

        sequence = sequence.Slice(reader.Position);

        return true;
    }
}