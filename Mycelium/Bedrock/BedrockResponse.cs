using System.Diagnostics.CodeAnalysis;

namespace Mycelium.Bedrock;

/// <summary>
/// Represents a Minecraft Bedrock edition server response.
/// </summary>
public sealed class BedrockResponse
{
    /// <summary>
    /// The two lines of the description (MOTD).
    /// </summary>
    // Change that to just two properties?
    public string[] Description { get; set; } =
    [
        string.Empty,
        string.Empty
    ];

    /// <summary>
    /// The server software's name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The server software's supported protocol version.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// The maximum count of players the server accepts. 
    /// </summary>
    public int Maximum { get; set; }

    /// <summary>
    /// The currently online count of players in the server.
    /// </summary>
    public int Online { get; set; }

    /// <summary>
    /// Converts the <see cref="string"/> representation of an <see cref="BedrockResponse"/> to the equivalent <see cref="BedrockResponse"/> structure.
    /// </summary>
    /// <param name="input">A <see cref="ReadOnlySpan{T}"/> containing the UTF-8 characters representing the <see cref="BedrockResponse"/> to convert.</param>
    /// <param name="response">Contains the converted <see cref="BedrockResponse"/> equivalent</param>
    /// <returns><c>true</c> if <paramref name="input"/> was converted successfully; otherwise, <c>false</c>.</returns>
    public static bool TryCreate(ReadOnlySpan<char> input, [NotNullWhen(true)] out BedrockResponse? response)
    {
        response = new BedrockResponse();

        var parts = input.Split(';');
        var index = 0;

        foreach (var part in parts)
        {
            switch (index)
            {
                case 1:
                    response.Description[0] = input[part].ToString();
                    break;

                case 2:
                    if (!int.TryParse(input[part], out var version))
                    {
                        return false;
                    }

                    response.Version = version;

                    break;

                case 3:
                    response.Name = input[part].ToString();
                    break;

                case 4:
                    if (!int.TryParse(input[part], out var online))
                    {
                        return false;
                    }

                    response.Online = online;

                    break;

                case 5:
                    if (!int.TryParse(input[part], out var maximum))
                    {
                        return false;
                    }

                    response.Maximum = maximum;

                    break;

                case 7:
                    response.Description[1] = input[part].ToString();
                    break;
            }

            index++;
        }

        // Have all parts been read?
        return index > 7;
    }
}