namespace Mycelium.Java;

/// <summary>
/// Represents a Minecraft server status.
/// </summary>
/// <param name="name">Server software's name.</param>
/// <param name="version">Supported protocol version.</param>
/// <param name="maximum">Maximum player count.</param>
/// <param name="online">Current online player count.</param>
public sealed class StatusResponse(string name, int version, int maximum, int online)
{
    /// <summary>
    /// Server software's name.
    /// </summary>
    /// <example>Bukkit</example>
    public string Name { get; } = name;

    /// <summary>
    /// Supported protocol version.
    /// </summary>
    /// <example>47</example>
    public int Version { get; } = version;

    /// <summary>
    /// Maximum player count.
    /// </summary>
    public int Maximum { get; } = maximum;

    /// <summary>
    /// Current online player count.
    /// </summary>
    public int Online { get; } = online;

    /// <summary>
    /// Creates a <see cref="StatusResponse"/> from a Minecraft server's status message.
    /// </summary>
    /// <param name="input">The Minecraft server status message.</param>
    /// <returns>The created <see cref="StatusResponse"/>.</returns>
    /// <remarks>https://minecraft.wiki/w/Minecraft_Wiki:Projects/wiki.vg_merge/Server_List_Ping#Status_Response</remarks>
    public static StatusResponse Create(string input)
    {
        return new StatusResponse("Hello, world!", 47, 10, 0);
    }
}