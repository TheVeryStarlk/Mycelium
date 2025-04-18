namespace Mycelium.Java;

internal readonly ref struct Address(ReadOnlySpan<char> first, ushort port)
{
    public ReadOnlySpan<char> First { get; } = first;

    public ushort Port { get; } = port;

    public static bool TryParse(string input, out Address address)
    {
        const char separator = ':';

        address = default;

        var span = input.AsSpan();

        if (span.Count(separator) > 1)
        {
            return false;
        }

        var index = span.IndexOf(separator) + 1;

        if (index >= span.Length)
        {
            return false;
        }

        if (!ushort.TryParse(span[index..], out var port) || index - 1 < 0)
        {
            return false;
        }

        var slice = span[..(index - 1)];

        if (slice.Length < 1)
        {
            return false;
        }

        address = new Address(slice, port);

        return true;
    }
}