using System.Buffers;
using System.IO.Pipelines;
using LightResults;

namespace Mycelium.Features.Java.Packets;

/// <summary>
/// Represents a Minecraft status response packet.
/// </summary>
internal static class StatusResponsePacket
{
    /// <summary>
    /// Reads a status response.
    /// </summary>
    /// <param name="input">The <see cref="PipeReader"/> to read from.</param>
    /// <param name="token">A <see cref="CancellationToken"/> that can be used to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous read operation.</returns>
    /// <exception cref="InvalidDataException">Incomplete packet.</exception>
    public static async ValueTask<Result<ReadOnlySequence<byte>>> ReadAsync(PipeReader input, CancellationToken token)
    {
        while (true)
        {
            var result = await input.ReadAsync(token);
            var buffer = result.Buffer;

            var consumed = buffer.Start;
            var examined = buffer.End;

            if (TryRead(ref buffer, out var status))
            {
                return status;
            }

            if (result.IsCompleted)
            {
                if (buffer.Length > 0)
                {
                    return Result.Failure<ReadOnlySequence<byte>>("Incomplete packet.");
                }

                break;
            }

            input.AdvanceTo(consumed, examined);
        }

        return Result.Failure<ReadOnlySequence<byte>>("Failed to read the packet.");
    }

    /// <summary>
    /// Tries to read a response from a <see cref="ReadOnlySequence{T}"/>.
    /// </summary>
    /// <param name="sequence">The <see cref="ReadOnlySequence{T}"/> to read from.</param>
    /// <param name="response">The read response.</param>
    /// <returns>True if the response was converted successfully, otherwise, false.</returns>
    private static bool TryRead(ref ReadOnlySequence<byte> sequence, out ReadOnlySequence<byte> response)
    {
        var reader = new SequenceReader<byte>(sequence);

        response = default;

        if (!reader.TryReadVariableInteger(out _) || !reader.TryReadVariableInteger(out var identifier)
                                                  || !reader.TryReadVariableString(out response)
                                                  || identifier != 0)
        {
            return false;
        }

        sequence = sequence.Slice(reader.Position);

        return true;
    }
}