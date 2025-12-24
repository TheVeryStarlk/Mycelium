namespace Mycelium;

public readonly struct Address(string host, ushort port)
{
    public string Host => host;

    public ushort Port => port;

    public static bool TryParse(ReadOnlySpan<char> input, out Address address)
    {
        address = default;

        // Arbitrary limit from Minecraft.
        if (input.Length > 64)
        {
            return false;
        }

        var separator = input.IndexOf(':');

        if (separator < 1)
        {
            return false;
        }

        if (!ushort.TryParse(input[(separator + 1)..], out var port))
        {
            return false;
        }

        address = new Address(input[..separator].ToString(), port);

        return true;
    }
}