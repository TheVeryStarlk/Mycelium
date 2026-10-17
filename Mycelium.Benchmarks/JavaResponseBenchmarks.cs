using BenchmarkDotNet.Attributes;
using Mycelium.Java;

namespace Mycelium.Benchmarks;

public class JavaResponseBenchmarks
{
    [Benchmark]
    public JavaResponse Creation()
    {
        const string value = """{"version":{"name":"1.21.2","protocol":768},"players":{"max":100,"online":5},"description":{"text":"Hello, world!"}}""";
        
        _ = JavaResponse.TryCreate(value, JavaVersion.Modern, out var response);

        return response!;
    }

    [Benchmark]
    public JavaResponse CreationLegacy()
    {
        const string value = "§1\078\01.6.4\0A Minecraft Server\05\020";

        _ = JavaResponse.TryCreate(value, JavaVersion.LegacyHost, out var response);

        return response!;
    }

    [Benchmark]
    public JavaResponse CreationBeta()
    {
        const string value = "§aColorful §bBeta Server§3§16";

        _ = JavaResponse.TryCreate(value, JavaVersion.LegacyBeta, out var response);

        return response!;
    }
}