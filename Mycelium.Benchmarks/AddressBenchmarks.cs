using BenchmarkDotNet.Attributes;

namespace Mycelium.Benchmarks;

public class AddressBenchmarks
{
    [Benchmark]
    public Address Parsing()
    {
        _ = Address.TryParse("foo.bar.baz.qux.:25565", out var address);
        return address;
    }
    
    [Benchmark]
    public Address ParsingShort()
    {
        _ = Address.TryParse("hello.world:25565", out var address);
        return address;
    }
}