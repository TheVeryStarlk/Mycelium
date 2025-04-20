using System.Diagnostics.CodeAnalysis;

namespace Mycelium.Features.Bedrock;

/// <summary>
/// Represents a Minecraft server status.
/// </summary>
/// <param name="description">Server's two lines of message of the day as known as MOTD.</param>
/// <param name="name">Server software's name.</param>
/// <param name="version">Supported protocol version.</param>
/// <param name="maximum">Maximum player count.</param>
/// <param name="online">Current online player count.</param>
internal sealed class BedrockResponse(string[] description, string? name, int version, int maximum, int online)
{
    /// <summary>
    /// Server's two lines of message of the day as known as MOTD.
    /// </summary>
    public string[] Description => description;

    /// <summary>
    /// Server software's name.
    /// </summary>
    public string? Name => name;

    /// <summary>
    /// Supported protocol version.
    /// </summary>
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
    /// Tries to create a <see cref="BedrockResponse"/> from a <see cref="string"/>.
    /// </summary>
    /// <param name="input">The <see cref="string"/> to read from.</param>
    /// <param name="response">The result <see cref="BedrockResponse"/>.</param>
    /// <returns>True if the <see cref="string"/> was converted successfully, otherwise, false.</returns>
    public static bool TryCreate(string input, [NotNullWhen(true)] out BedrockResponse? response)
    {
        response = null;

        // Rewrite to use spans.
        var parts = input.Split(';');

        if (parts.Length < 8)
        {
            return false;
        }

        if (!int.TryParse(parts[2], out var version) || !int.TryParse(parts[4], out var online)
                                                     || !int.TryParse(parts[5], out var maximum))
        {
            return false;
        }

        string[] description =
        [
            parts[0],
            parts[7]
        ];

        response = new BedrockResponse(description, parts[3], version, maximum, online);
        return true;
    }
}