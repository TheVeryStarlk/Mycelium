using Microsoft.Extensions.Caching.Memory;

namespace Mycelium.Features.Bedrock;

internal sealed class BedrockClient(ILogger<BedrockClient> logger, IMemoryCache cache)
{
    private const string Prefix = "Bedrock";
}