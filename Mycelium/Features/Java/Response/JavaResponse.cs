using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Mycelium.Features.Java.Response;

/// <summary>
/// Represents a Minecraft Java server status.
/// </summary>
/// <param name="description">Server's message of the day as known as MOTD.</param>
/// <param name="name">Server software's name.</param>
/// <param name="version">Supported protocol version.</param>
/// <param name="maximum">Maximum player count.</param>
/// <param name="online">Current online player count.</param>
internal sealed class JavaResponse(string? description, string? name, int version, int maximum, int online)
{
    private static JsonSerializerOptions options = new()
    {
        PropertyNameCaseInsensitive = true,
        TypeInfoResolver = MyceliumJsonSerializerContext.Default
    };

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
    /// Tries to create a <see cref="JavaResponse"/> from a <see cref="ReadOnlySequence{T}"/>.
    /// </summary>
    /// <param name="input">The <see cref="ReadOnlySequence{T}"/> to read from.</param>
    /// <param name="response">The result <see cref="JavaResponse"/>.</param>
    /// <returns>True if the <see cref="ReadOnlySequence{T}"/> was converted successfully, otherwise, false.</returns>
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
        // Use JsonNode serialization or Utf8JsonReader?
        try
        {
            var reader = new Utf8JsonReader(input);

            var result = JsonSerializer.Deserialize<JavaStatus>(ref reader, options);

            response = new JavaResponse(
                result!.Description,
                result.Version.Name,
                result.Version.Protocol,
                result.Players.Maximum,
                result.Players.Online);

            return true;
        }
        catch (JsonException)
        {
            response = null;
            return false;
        }
    }
}