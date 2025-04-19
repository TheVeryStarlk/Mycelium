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
    /// <param name="edition">The Minecraft <see cref="Edition"/>.</param>
    /// <param name="input">The <see cref="string"/> to read from.</param>
    /// <param name="response">The result <see cref="StatusResponse"/>.</param>
    /// <returns>True if the <see cref="string"/> was converted successfully, otherwise, false.</returns>
    public static bool TryCreate(Edition edition, string input, [NotNullWhen(true)] out StatusResponse? response)
    {
        response = null;

        // Probably should split this class into Java and Bedrock editions.
        switch (edition)
        {
            case Edition.Java:
                // Refactor this to use JSON reader.
                var node = JsonNode.Parse(input)!;

                response = new StatusResponse(
                    node["description"]?.ToString(),
                    node["version"]?["name"]?.ToString(),
                    node["version"]?["protocol"]?.GetValue<int>() ?? 0,
                    node["players"]?["max"]?.GetValue<int>() ?? 0,
                    node["players"]?["online"]?.GetValue<int>() ?? 0);

                return true;

            // Edition;MOTD line 1     ;Protocol Version;Version Name;Player Count;Max Player Count;Server Unique ID;MOTD line 2
            // MCPE   ;Dedicated Server;390             ;1.14.60     ;0           ;10              ;132538608923865 ;Bedrock level

            case Edition.Bedrock:
                // Rewrite to use spans.
                var parts = input.Split(';');

                if (parts.Length < 8)
                {
                    return false;
                }

                if (!int.TryParse(parts[2], out var version)
                    || !int.TryParse(parts[4], out var online)
                    || !int.TryParse(parts[5], out var maximum))
                {
                    return false;
                }

                response = new StatusResponse(parts[0] + parts[7], parts[3], version, maximum, online);
                return true;
        }

        return false;
    }
}