using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mycelium.Java;

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

    public string? Description { get; set; }

    public string? Name { get; set; }

    public int Version { get; set; }

    public int Maximum { get; set; }

    public int Online { get; set; }

    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
        Justification = "Referenced in MyceliumJsonSerializerContext")]
    [UnconditionalSuppressMessage(
        "AOT",
        "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.",
        Justification = "Referenced in MyceliumJsonSerializerContext")]
    public static bool TryCreate(ReadOnlySequence<byte> input, [NotNullWhen(true)] out JavaResponse? response)
    {
        // Use JsonNode deserialization or Utf8JsonReader?
        try
        {
            var reader = new Utf8JsonReader(input);

            var result = JsonSerializer.Deserialize<Status>(ref reader, options);

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