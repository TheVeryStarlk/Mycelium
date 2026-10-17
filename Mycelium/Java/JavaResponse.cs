using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mycelium.Java;

/// <summary>
/// Represents a Minecraft Java edition server response.
/// </summary>
public sealed class JavaResponse
{
    internal sealed class Status
    {
        public sealed class Server
        {
            public required string Name { get; init; }

            public required int Protocol { get; init; }
        }

        public sealed class Information
        {
            public required int Online { get; init; }

            [JsonPropertyName("max")]
            public required int Maximum { get; init; }
        }

        public required Server Version { get; init; }

        public required Information Players { get; init; }

        [JsonConverter(typeof(DescriptionConverter))]
        public required string Description { get; init; }
    }

    private static JsonSerializerOptions options = new()
    {
        PropertyNameCaseInsensitive = true,
        TypeInfoResolver = MyceliumJsonSerializerContext.Default
    };

    /// <summary>
    /// The description (MOTD).
    /// </summary>
    /// <remarks>
    /// Modern servers describe it as raw JSON text, while legacy servers describe it as plain text that may contain '§' formatting codes.
    /// </remarks>
    public string? Description { get; set; }

    /// <summary>
    /// The server software's name.
    /// </summary>
    /// <remarks>
    /// This is <c>null</c> for <see cref="JavaVersion.LegacyBeta"/> servers, as they do not report it.
    /// </remarks>
    public string? Name { get; set; }

    /// <summary>
    /// The server software's supported protocol version.
    /// </summary>
    /// <remarks>
    /// This is <c>-1</c> for <see cref="JavaVersion.LegacyBeta"/> servers, as they do not report it.
    /// </remarks>
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
    /// Converts the <see cref="string"/> representation of an <see cref="JavaResponse"/> to the equivalent <see cref="JavaResponse"/> structure.
    /// </summary>
    /// <param name="input">A <see cref="ReadOnlySpan{T}"/> containing the characters representing the <see cref="JavaResponse"/> to convert.</param>
    /// <param name="version">
    /// The <see cref="JavaVersion"/> that <paramref name="input"/> was requested with, which determines its format:
    /// JSON for <see cref="JavaVersion.Modern"/>, "§1\0"-prefixed for <see cref="JavaVersion.LegacyHost"/> and <see cref="JavaVersion.LegacyExtended"/>,
    /// and '§'-delimited for <see cref="JavaVersion.LegacyBeta"/>.
    /// </param>
    /// <param name="response">Contains the converted <see cref="JavaResponse"/> equivalent</param>
    /// <returns><c>true</c> if <paramref name="input"/> was converted successfully; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="version"/> is not a defined <see cref="JavaVersion"/>.</exception>
    public static bool TryCreate(ReadOnlySpan<char> input, JavaVersion version, [NotNullWhen(true)] out JavaResponse? response)
    {
        return version switch
        {
            JavaVersion.Modern => TryParseModern(input, out response),
            JavaVersion.LegacyHost or JavaVersion.LegacyExtended => TryParseLegacy(input, out response),
            JavaVersion.LegacyBeta => TryParseBeta(input, out response),
            _ => throw new ArgumentOutOfRangeException(nameof(version), version, null)
        };
    }

    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
        Justification = "Referenced in MyceliumJsonSerializerContext")]
    [UnconditionalSuppressMessage(
        "AOT",
        "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.",
        Justification = "Referenced in MyceliumJsonSerializerContext")]
    private static bool TryParseModern(ReadOnlySpan<char> input, [NotNullWhen(true)] out JavaResponse? response)
    {
        // Use JsonNode deserialization or Utf8JsonReader?
        try
        {
            var result = JsonSerializer.Deserialize<Status>(input, options);

            response = new JavaResponse
            {
                Description = result!.Description,
                Name = result.Version.Name,
                Version = result.Version.Protocol,
                Maximum = result.Players.Maximum,
                Online = result.Players.Online
            };

            return true;
        }
        catch (JsonException)
        {
            response = null;
            return false;
        }
    }

    private static bool TryParseLegacy(ReadOnlySpan<char> input, [NotNullWhen(true)] out JavaResponse? response)
    {
        response = null;

        if (!input.StartsWith("§1\0", StringComparison.Ordinal))
        {
            return false;
        }

        // Protocol, name, description, online and maximum, separated by null characters.
        input = input[3..];

        var parts = input.Split('\0');
        var index = 0;

        var version = 0;
        var online = 0;
        var maximum = 0;

        ReadOnlySpan<char> name = default;
        ReadOnlySpan<char> description = default;

        foreach (var part in parts)
        {
            switch (index)
            {
                case 0:
                    if (!int.TryParse(input[part], out version))
                    {
                        return false;
                    }

                    break;

                case 1:
                    name = input[part];
                    break;

                case 2:
                    description = input[part];
                    break;

                case 3:
                    if (!int.TryParse(input[part], out online))
                    {
                        return false;
                    }

                    break;

                case 4:
                    if (!int.TryParse(input[part], out maximum))
                    {
                        return false;
                    }

                    break;

                default:
                    return false;
            }

            index++;
        }

        // Have all parts been read?
        if (index != 5)
        {
            return false;
        }

        response = new JavaResponse
        {
            Description = description.ToString(),
            Name = name.ToString(),
            Version = version,
            Maximum = maximum,
            Online = online
        };

        return true;
    }

    private static bool TryParseBeta(ReadOnlySpan<char> input, [NotNullWhen(true)] out JavaResponse? response)
    {
        response = null;

        // Description, online and maximum, separated by '§'. Parsed from the right as the description may contain '§' itself.
        var separator = input.LastIndexOf('§');

        if (separator < 0 || !int.TryParse(input[(separator + 1)..], out var maximum))
        {
            return false;
        }

        input = input[..separator];
        separator = input.LastIndexOf('§');

        if (separator < 0 || !int.TryParse(input[(separator + 1)..], out var online))
        {
            return false;
        }

        response = new JavaResponse
        {
            Description = input[..separator].ToString(),
            Name = null,
            Version = -1,
            Maximum = maximum,
            Online = online
        };

        return true;
    }
}