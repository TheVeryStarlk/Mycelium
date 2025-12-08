using System.Net.Sockets;
using System.Text;
using CommunityToolkit.HighPerformance.Buffers;
using LightResults;

namespace Mycelium.Features.Bedrock.Packets;

/// <summary>
/// Represents a Minecraft Bedrock unconnected pong packet.
/// </summary>
internal static class UnconnectedPongPacket
{
    /// <summary>
    /// Reads an unconnected pong's status response.
    /// </summary>
    /// <param name="socket">The <see cref="Socket"/> to read from.</param>
    /// <param name="token">A <see cref="CancellationToken"/> that can be used to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="Result"/> containing the read status.</returns>
    public static async Task<Result<string>> ReadAsync(Socket socket, CancellationToken token)
    {
        // Maximum transmission unit.
        using var owner = MemoryOwner<byte>.Allocate(1500);

        var received = 0;

        try
        {
            received = await socket.ReceiveAsync(owner.Memory, token);
        }
        catch (OperationCanceledException)
        {
            return Result.Failure<string>();
        }

        // Identifier, two longs, the magic and the string's unsigned short prefix.
        // Probably should validate the payload too.
        const byte skip = 35;

        return received > skip
            ? Result.Success(Encoding.UTF8.GetString(owner.Span[skip..received]))
            : Result.Failure<string>("Incomplete packet.");
    }
}