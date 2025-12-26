using BenchmarkDotNet.Attributes;
using Mycelium.Java.Packets;

namespace Mycelium.Benchmarks;

public sealed class VariableLengthBenchmarks
{
    [Benchmark]
    [Arguments(256)]
    [Arguments(65536)]
    [Arguments(16777216)]
    [Arguments(2147483647)]
    public int GetIntegerByteCount(int value)
    {
        return VariableLength.GetByteCount(value);
    }
    
    [Benchmark]
    [Arguments("Foo")]
    [Arguments("Foo Bar")]
    [Arguments("Foo Bar Baz")]
    [Arguments("Foo Bar Baz Qux")]
    public int GetIntegerByteCount(string value)
    {
        return VariableLength.GetByteCount(value);
    }

    [Benchmark]
    [Arguments(256)]
    [Arguments(65536)]
    [Arguments(16777216)]
    [Arguments(2147483647)]
    public int WriteInteger(int value)
    {
        Span<byte> span = stackalloc byte[sizeof(int) + sizeof(byte)];
        return VariableLength.Write(span, value);
    }
    
    [Benchmark]
    [Arguments("Foo")]
    [Arguments("Foo Bar")]
    [Arguments("Foo Bar Baz")]
    [Arguments("Foo Bar Baz Qux")]
    public int WriteString(string value)
    {
        return VariableLength.GetByteCount(value);
    }
}