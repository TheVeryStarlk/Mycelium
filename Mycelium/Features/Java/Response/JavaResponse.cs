using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Mycelium.Features.Java.Response;

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
    public static bool TryCreate(ReadOnlySequence<byte> input, [NotNullWhen(true)] out JavaResponse? response)
    {
        var reader = new Utf8JsonReader(input);

#pragma warning disable IL2026
#pragma warning disable IL3050
        var result = JsonSerializer.Deserialize<JavaStatus>(ref reader, options);
#pragma warning restore IL3050
#pragma warning restore IL2026

        response = new JavaResponse(
            result!.Description,
            result.Version.Name,
            result.Version.Protocol,
            result.Players.Maximum,
            result.Players.Online);

        return true;
    }
}