using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.HighPerformance;

namespace Mycelium.Features.Bedrock;

/// <summary>
/// Represents a Minecraft Bedrock server status.
/// </summary>
internal sealed class BedrockResponse
{
    /// <summary>
    /// Server's two lines of message of the day as known as MOTD.
    /// </summary>
    public string[] Description { get; set; } =
    [
        string.Empty,
        string.Empty
    ];

    /// <summary>
    /// Server software's name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Supported protocol version.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Maximum player count.
    /// </summary>
    public int Maximum { get; set; }

    /// <summary>
    /// Current online player count.
    /// </summary>
    public int Online { get; set; }

    /// <summary>
    /// Tries to create a <see cref="BedrockResponse"/> from a <see cref="string"/>.
    /// </summary>
    /// <param name="input">The <see cref="string"/> to read from.</param>
    /// <param name="response">The result <see cref="BedrockResponse"/>.</param>
    /// <returns>True if the <see cref="string"/> was converted successfully, otherwise, false.</returns>
    public static bool TryCreate(ReadOnlySpan<char> input, [NotNullWhen(true)] out BedrockResponse? response)
    {
        response = new BedrockResponse();

        const char separator = ';';

        var index = 0;

        foreach (var token in input.Tokenize(separator))
        {
            switch (index)
            {
                case 1:
                    response.Description[0] = token.ToString();
                    break;

                case 2:
                    if (!int.TryParse(token, out var version))
                    {
                        return false;
                    }

                    response.Version = version;

                    break;

                case 3:
                    response.Name = token.ToString();
                    break;

                case 4:
                    if (!int.TryParse(token, out var online))
                    {
                        return false;
                    }

                    response.Online = online;

                    break;

                case 5:
                    if (!int.TryParse(token, out var maximum))
                    {
                        return false;
                    }

                    response.Maximum = maximum;

                    break;

                case 7:
                    response.Description[1] = token.ToString();
                    break;
            }

            index++;
        }

        // Have all parts been read?
        return index > 7;
    }
}