using System.Text.Json.Serialization;
using Mycelium.Java;

namespace Mycelium;

[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(StatusResponse))]
internal sealed partial class MyceliumJsonSerializerContext : JsonSerializerContext;