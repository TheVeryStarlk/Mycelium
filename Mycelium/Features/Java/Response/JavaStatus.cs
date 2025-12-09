using System.Text.Json.Serialization;

namespace Mycelium.Features.Java.Response;

internal sealed class JavaStatus
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