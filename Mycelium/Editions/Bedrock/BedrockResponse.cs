using System.Diagnostics.CodeAnalysis;

namespace Mycelium.Editions.Bedrock;

public sealed class BedrockResponse
{
    public string[] Description { get; set; } =
    [
        string.Empty,
        string.Empty
    ];

    public string? Name { get; set; }

    public int Version { get; set; }

    public int Maximum { get; set; }

    public int Online { get; set; }

    public static bool TryCreate(ReadOnlySpan<char> input, [NotNullWhen(true)] out BedrockResponse? response)
    {
        response = null;
        return false;
    }
}