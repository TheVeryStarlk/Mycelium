using System.Text.Json.Serialization;
using Mycelium.Java;

namespace Mycelium;

[JsonSerializable(typeof(JavaResponse))]
[JsonSerializable(typeof(JavaResponse.Status))]
[JsonSerializable(typeof(JavaResponse.Status.Server))]
[JsonSerializable(typeof(JavaResponse.Status.Information))]
internal sealed partial class MyceliumJsonSerializerContext : JsonSerializerContext;