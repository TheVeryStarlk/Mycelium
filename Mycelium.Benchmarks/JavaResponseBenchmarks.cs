using BenchmarkDotNet.Attributes;
using Mycelium.Java;

namespace Mycelium.Benchmarks;

public class JavaResponseBenchmarks
{
    [Benchmark]
    public JavaResponse Creation()
    {
        const string value = """{"version":{"name":"1.21.2","protocol":768},"players":{"max":100,"online":5},"description":{"text":"Hello, world!"}}""";
        
        _ = JavaResponse.TryCreate(value, out var response);
        
        return response!;
    }
}