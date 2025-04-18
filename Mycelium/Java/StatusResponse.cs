namespace Mycelium.Java;

/// <summary>
/// Represents a Minecraft server status.
/// </summary>
/// <param name="description">Server's message of the day as known as MOTD.</param>
/// <param name="name">Server software's name.</param>
/// <param name="version">Supported protocol version.</param>
/// <param name="maximum">Maximum player count.</param>
/// <param name="online">Current online player count.</param>
internal sealed class StatusResponse(string description, string name, int version, int maximum, int online)
{
    /// <summary>
    /// Server's message of the day as known as MOTD.
    /// </summary>
    public string Description => description;

    /// <summary>
    /// Server software's name.
    /// </summary>
    /// <example>1.8.9</example>
    public string Name => name;

    /// <summary>
    /// Supported protocol version.
    /// </summary>
    /// <example>47</example>
    public int Version => version;

    /// <summary>
    /// Maximum player count.
    /// </summary>
    public int Maximum => maximum;

    /// <summary>
    /// Current online player count.
    /// </summary>
    public int Online => online;

    /// <summary>
    /// Creates a <see cref="StatusResponse"/> from a Minecraft server's status message.
    /// </summary>
    /// <param name="input">The Minecraft server status message.</param>
    /// <returns>The created <see cref="StatusResponse"/>.</returns>
    /// <remarks>https://minecraft.wiki/w/Minecraft_Wiki:Projects/wiki.vg_merge/Server_List_Ping#Status_Response</remarks>
    public static StatusResponse Create(ReadOnlySpan<byte> input)
    {
        return new StatusResponse(string.Empty, string.Empty, 0, 0, 0);
    }
}