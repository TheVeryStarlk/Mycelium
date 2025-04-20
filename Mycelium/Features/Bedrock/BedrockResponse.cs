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
    public static bool TryCreate(ReadOnlySpan<char> input, [NotNullWhen(true)] out BedrockResponse? response)
    {
        response = null;

        const char separator = ';';

        var count = input.Count(separator);

        if (count < 8)
        {
            return false;
        }

        var description = new string[2];
        var name = string.Empty;
        var version = 0;
        var maximum = 0;
        var online = 0;

        var parts = input.Split(separator);
        var index = 0;

        foreach (var part in parts)
        {
            switch (index)
            {
                case 1:
                    description[0] = input[part].ToString();
                    break;

                case 2:
                    if (!int.TryParse(input[part], out version))
                    {
                        return false;
                    }

                    break;

                case 3:
                    name = input[part].ToString();
                    break;

                case 4:
                    if (!int.TryParse(input[part], out online))
                    {
                        return false;
                    }

                    break;

                case 5:
                    if (!int.TryParse(input[part], out maximum))
                    {
                        return false;
                    }

                    break;

                case 7:
                    description[1] = input[part].ToString();
                    break;
            }

            index++;
        }

        response = new BedrockResponse(description, name, version, maximum, online);

        return true;
    }
}