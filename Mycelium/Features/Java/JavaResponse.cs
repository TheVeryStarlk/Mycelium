using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;

namespace Mycelium.Features.Java;

/// <summary>
/// Represents a Minecraft server status.
/// </summary>
/// <param name="description">Server's message of the day as known as MOTD.</param>
/// <param name="name">Server software's name.</param>
/// <param name="version">Supported protocol version.</param>
/// <param name="maximum">Maximum player count.</param>
/// <param name="online">Current online player count.</param>
internal sealed class JavaResponse(string? description, string? name, int version, int maximum, int online)
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
    /// Tries to create a <see cref="JavaResponse"/> from a <see cref="string"/>.
    /// </summary>
    /// <param name="input">The <see cref="string"/> to read from.</param>
    /// <param name="response">The result <see cref="JavaResponse"/>.</param>
    /// <returns>True if the <see cref="string"/> was converted successfully, otherwise, false.</returns>
    public static bool TryCreate(string input, [NotNullWhen(true)] out JavaResponse? response)
    {
        // Refactor this to use JSON reader.
        var node = JsonNode.Parse(input)!;

        response = new JavaResponse(
            node["description"]?.ToString(),
            node["version"]?["name"]?.ToString(),
            node["version"]?["protocol"]?.GetValue<int>() ?? 0,
            node["players"]?["max"]?.GetValue<int>() ?? 0,
            node["players"]?["online"]?.GetValue<int>() ?? 0);

        return true;
    }
}