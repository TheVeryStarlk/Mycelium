using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mycelium.Java;

internal sealed class DescriptionConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        return document.RootElement.GetRawText();
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        using var document = JsonDocument.Parse(value);
        document.RootElement.WriteTo(writer);
    }
}