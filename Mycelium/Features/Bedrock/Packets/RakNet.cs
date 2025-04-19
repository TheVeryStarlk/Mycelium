using System.Collections.Immutable;

namespace Mycelium.Features.Bedrock.Packets;

/// <summary>
/// Provides constants about the <see cref="RakNet"/> protocol.
/// </summary>
internal static class RakNet
{
    /// <summary>
    /// The unconnected ping packet.
    /// </summary>
    public static ImmutableArray<byte> UnconnectedPingPacket { get; } =
    [
        // Identifier
        1,

        // Time.
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,

        // Magic.
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

        // GUID.
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