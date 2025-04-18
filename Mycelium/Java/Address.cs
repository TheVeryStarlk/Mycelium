namespace Mycelium.Java;

internal readonly ref struct Address(ReadOnlySpan<char> first, ushort port)
{
    public ReadOnlySpan<char> First { get; } = first;

    public ushort Port { get; } = port;

    private const char Separator = ':';

    public static bool TryParse(string input, out Address address)
    {
        address = default;

        var span = input.AsSpan();

        if (span.Count(Separator) > 1)
        {
            return false;
        }

        var index = span.IndexOf(Separator);

        if (index + 1 >= span.Length)
        {
            return false;
        }

        index++;

        if (!ushort.TryParse(span[index..], out var port) || index - 1 < 0)
        {
            return false;
        }

        index--;

        var slice = span[..index];

        if (slice.Length < 1)
        {
            return false;
        }

        address = new Address(slice, port);

        return true;
    }
}