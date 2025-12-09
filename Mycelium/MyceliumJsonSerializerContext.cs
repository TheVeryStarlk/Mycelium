using System.Text.Json.Serialization;
using Mycelium.Features.Bedrock;
using Mycelium.Features.Java.Response;

namespace Mycelium;

[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(JavaResponse))]
[JsonSerializable(typeof(BedrockResponse))]
[JsonSerializable(typeof(JavaStatus))]
[JsonSerializable(typeof(JavaStatus.Server))]
[JsonSerializable(typeof(JavaStatus.Information))]
internal sealed partial class MyceliumJsonSerializerContext : JsonSerializerContext;