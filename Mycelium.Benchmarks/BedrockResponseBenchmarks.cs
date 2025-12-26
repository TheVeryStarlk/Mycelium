using BenchmarkDotNet.Attributes;
using Mycelium.Bedrock;

namespace Mycelium.Benchmarks;

public sealed class BedrockResponseBenchmarks
{
    [Benchmark]
    public BedrockResponse Creation()
    {
        const string value = "MCPE;Dedicated Server;390;1.14.60;0;10;13253860892328930865;Bedrock level;Survival;1;19132;19133;";
        
        _ = BedrockResponse.TryCreate(value, out var response);
        
        return response!;
    }
}