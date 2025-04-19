using System.Collections.Immutable;

namespace Mycelium.Features.Bedrock.Packets;

internal static class RakNet
{
    /// <summary>
    /// The unconnected ping packet in <see cref="RakNet"/> hard-coded.
    /// </summary>
    /// <remarks>
    /// doesn't allocate, see <see href="https://github.com/dotnet/roslyn/pull/24621"/>.
    /// </remarks>
    public static ImmutableArray<byte> UnconnectedPingPacket { get; } =
    [
        1,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        255,
        255,
        0,
        254,
        254,
        254,
        254,
        253,
        253,
        253,
        253,
        18,
        52,
        86,
        120,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0
    ];

    // https://github.com/vp817/RakNetProtocolDoc?tab=readme-ov-file#general-constants.
    public const short MaximumTransmissionUnit = 1492;
}