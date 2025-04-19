using System.Net.Sockets;
using System.Text;
using CommunityToolkit.HighPerformance.Buffers;
using LightResults;

namespace Mycelium.Features.Bedrock.Packets;

internal static class UnconnectedPongPacket
{
    public static async Task<Result<string>> ReadAsync(Socket connection)
    {
        using var owner = MemoryOwner<byte>.Allocate(RakNet.MaximumTransmissionUnit);

        var received = await connection.ReceiveAsync(owner.Memory);

        // Two longs, the magic and the string's unsigned short prefix.
        const byte skip = 34;

        return received < skip
            ? Result.Failure<string>("Incomplete packet.")
            : Result.Success(Encoding.UTF8.GetString(owner.Span[skip..received]));

    }
}