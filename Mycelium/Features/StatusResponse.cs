using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;

namespace Mycelium.Features;

/// <summary>
/// Represents a Minecraft server status.
/// </summary>
/// <param name="description">Server's message of the day as known as MOTD.</param>
/// <param name="name">Server software's name.</param>
/// <param name="version">Supported protocol version.</param>
/// <param name="maximum">Maximum player count.</param>
/// <param name="online">Current online player count.</param>
internal sealed class StatusResponse(string? description, string? name, int version, int maximum, int online)
{
    /// <summary>
    /// Server's message of the day as known as MOTD.
    /// </summary>
    public string? Description => description;

    /// <summary>
    /// Server software's name.
    /// </summary>
    /// <example>1.8.9</example>
    public string? Name => name;

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
    /// Tries to read a <see cref="StatusResponse"/> from a <see cref="string"/>.
    /// </summary>
    /// <param name="input">The <see cref="string"/> to read from.</param>
    /// <param name="response">The result <see cref="StatusResponse"/>.</param>
    /// <returns>True if the <see cref="string"/> was converted successfully, otherwise, false.</returns>
    public static bool TryCreate(string input, [NotNullWhen(true)] out StatusResponse? response)
    {
        // Refactor this to use JSON reader.
        var node = JsonNode.Parse(input)!;

        var version = node["version"];
        var players = node["players"];
        var description = node["description"];

        response = new StatusResponse(
            description?.ToString(),
            version?["name"]?.ToString(),
            version?["protocol"]?.GetValue<int>() ?? 0,
            players?["max"]?.GetValue<int>() ?? 0,
            players?["online"]?.GetValue<int>() ?? 0);

        return true;
    }
}