using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using CommunityToolkit.HighPerformance.Buffers;

namespace Mycelium.Features.Java;

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
    /// Tries to create a <see cref="JavaResponse"/> from a <see cref="string"/>.
    /// </summary>
    /// <param name="input">The <see cref="ReadOnlySpan{T}"/> to read from.</param>
    /// <param name="response">The result <see cref="JavaResponse"/>.</param>
    /// <returns>True if the <see cref="string"/> was converted successfully, otherwise, false.</returns>
    public static bool TryCreate(ReadOnlySpan<char> input, [NotNullWhen(true)] out JavaResponse? response)
    {
        response = null;

        var description = string.Empty;
        var name = string.Empty;
        var version = 0;
        var maximum = 0;
        var online = 0;

        using var owner = SpanOwner<byte>.Allocate(Encoding.UTF8.GetByteCount(input));
        Debug.Assert(owner.Length >= Encoding.UTF8.GetBytes(input, owner.Span));

        var reader = new Utf8JsonReader(owner.Span);

        while (reader.Read())
        {
            if (reader.TokenType is not JsonTokenType.PropertyName)
            {
                continue;
            }

            var property = reader.GetString();

            if (!reader.Read())
            {
                return false;
            }

            switch (property)
            {
                case "name":
                    name = reader.GetString();
                    break;

                case "protocol":
                    version = reader.GetInt32();
                    break;

                case "max":
                    maximum = reader.GetInt32();
                    break;

                case "online":
                    online = reader.GetInt32();
                    break;

                case "description":
                    if (reader.TokenType is JsonTokenType.String)
                    {
                        description = reader.GetString();
                        break;
                    }

                    // Capture the starting brace.
                    var old = reader.BytesConsumed - 1;

                    if (!reader.TrySkip())
                    {
                        return false;
                    }

                    // Capture the ending brace.
                    var last = reader.BytesConsumed + 1;

                    // Ideally, description should be parsed as a Minecraft component, but for now, this reads the entire description's property as a string and returns it.
                    // Because some servers return "cursed" JSON that confuses the reader, the end result is sliced to remove anything that is outside the description property.
                    var slice = input[(int) old..(int) last];
                    description = slice[..(slice.LastIndexOf('}') + 1)].ToString();

                    break;
            }
        }

        response = new JavaResponse(description, name, version, maximum, online);

        return true;
    }
}