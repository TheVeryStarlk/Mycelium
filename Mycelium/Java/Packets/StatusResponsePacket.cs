using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipelines;
using LightResults;

namespace Mycelium.Java.Packets;

/// <summary>
/// Represents a Minecraft status response packet.
/// </summary>
internal static class StatusResponsePacket
{
    /// <summary>
    /// Reads a status response packet.
    /// </summary>
    /// <param name="input">The <see cref="PipeReader"/> to read from.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous read operation.</returns>
    /// <exception cref="InvalidDataException">Incomplete packet.</exception>
    public static async ValueTask<Result<string>> ReadAsync(PipeReader input)
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
                        return Result.Failure<string>("Incomplete packet.");
                    }

                    break;
                }
            }
            finally
            {
                input.AdvanceTo(consumed, examined);
            }
        }

        return Result.Failure<string>("Failed to read the packet.");
    }

    /// <summary>
    /// Tries to read a response from a <see cref="ReadOnlySequence{T}"/>.
    /// </summary>
    /// <param name="sequence">The <see cref="ReadOnlySequence{T}"/> to read from.</param>
    /// <param name="response">The read response.</param>
    /// <returns>True if the response was converted successfully, otherwise, false.</returns>
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