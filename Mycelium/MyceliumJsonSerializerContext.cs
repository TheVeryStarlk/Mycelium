using System.Text.Json.Serialization;
using Mycelium.Features.Bedrock;
using Mycelium.Features.Java;

namespace Mycelium;

[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(JavaResponse))]
[JsonSerializable(typeof(BedrockResponse))]
internal sealed partial class MyceliumJsonSerializerContext : JsonSerializerContext;