using System.Text.Json.Serialization;

namespace Mycelium;

[JsonSerializable(typeof(string))]
internal sealed partial class MyceliumJsonSerializerContext : JsonSerializerContext;