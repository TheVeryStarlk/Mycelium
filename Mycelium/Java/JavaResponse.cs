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
    public string? Description { get; set; }

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
    /// Converts the <see cref="string"/> representation of an <see cref="JavaResponse"/> to the equivalent <see cref="JavaResponse"/> structure.
    /// </summary>
    /// <param name="input">A <see cref="ReadOnlySpan{T}"/> containing the UTF-8 characters representing the <see cref="JavaResponse"/> to convert.</param>
    /// <param name="response">Contains the converted <see cref="JavaResponse"/> equivalent</param>
    /// <returns><c>true</c> if <paramref name="input"/> was converted successfully; otherwise, <c>false</c>.</returns>
    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
        Justification = "Referenced in MyceliumJsonSerializerContext")]
    [UnconditionalSuppressMessage(
        "AOT",
        "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.",
        Justification = "Referenced in MyceliumJsonSerializerContext")]
    public static bool TryCreate(ReadOnlySpan<char> input, [NotNullWhen(true)] out JavaResponse? response)
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
}