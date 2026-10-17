using System.Buffers;
using System.IO.Pipelines;
using System.Text;

namespace Mycelium.Java.Packets;

internal static class LegacyStatusResponsePacket
{
    public static async ValueTask<string> ReadAsync(PipeReader input, CancellationToken token)
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
                    return Encoding.BigEndianUnicode.GetString(status);
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

        throw new MyceliumException("Failed to read packet.");
    }

    private static bool TryRead(ref ReadOnlySequence<byte> sequence, out ReadOnlySequence<byte> status)
    {
        var reader = new SequenceReader<byte>(sequence);

        status = default;

        // Kick packet.
        if (!reader.TryRead(out var identifier) || identifier != 255 || !reader.TryReadBigEndianString(out status))
        {
            return false;
        }

        sequence = sequence.Slice(reader.Position);

        return true;
    }
}