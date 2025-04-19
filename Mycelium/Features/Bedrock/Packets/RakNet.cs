using System.Collections.Immutable;

namespace Mycelium.Features.Bedrock.Packets;

/// <summary>
/// Provides constants about the <see cref="RakNet"/> protocol.
/// </summary>
internal static class RakNet
{
    /// <summary>
    /// The unconnected ping packet in <see cref="RakNet"/> hard-coded.
    /// </summary>
    /// <remarks>
    /// doesn't allocate, <see href="https://github.com/dotnet/roslyn/pull/24621"/>.
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

    /// <summary>
    /// The maximum transmission unit size in <see cref="byte"/>s.
    /// </summary>
    /// <remarks>
    /// <see href="https://github.com/vp817/RakNetProtocolDoc?tab=readme-ov-file#general-constants."/>.
    /// </remarks>
    public const short MaximumTransmissionUnit = 1492;
}