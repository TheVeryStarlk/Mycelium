namespace Mycelium.Java;

/// <summary>
/// Represents a Minecraft Java edition server list ping protocol version.
/// </summary>
public enum JavaVersion
{
    /// <summary>
    /// The Netty-based protocol used by 1.7 and later, which responds with a JSON status.
    /// </summary>
    Modern,

    /// <summary>
    /// The legacy protocol used by 1.6, which sends a server list ping followed by an "MC|PingHost" plugin message.
    /// </summary>
    LegacyHost,

    /// <summary>
    /// The legacy protocol used by 1.4 and 1.5, which sends a server list ping with a payload requesting extended information.
    /// </summary>
    LegacyExtended,

    /// <summary>
    /// The legacy protocol used by Beta 1.8 through 1.3, which sends a bare server list ping.
    /// </summary>
    /// <remarks>
    /// Servers using this version do not report their software's name nor protocol version.
    /// </remarks>
    LegacyBeta
}
