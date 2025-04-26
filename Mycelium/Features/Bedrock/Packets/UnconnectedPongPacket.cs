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
    /// <param name="connection">The <see cref="Socket"/> to read from.</param>
    /// <param name="token">A <see cref="CancellationToken"/> that can be used to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="Result"/> containing the read status.</returns>
    public static async Task<Result<string>> ReadAsync(Socket connection, CancellationToken token)
    {
        // Maximum transmission unit.
        using var owner = MemoryOwner<byte>.Allocate(1500);

        var received = await connection.ReceiveAsync(owner.Memory, token);

        // Two longs, the magic and the string's unsigned short prefix.
        // Probably should validate the payload, though.
        const byte skip = 34;

        return received > skip
            ? Result.Success(Encoding.UTF8.GetString(owner.Span[skip..received]))
            : Result.Failure<string>("Incomplete packet.");
    }
}