namespace Mycelium;

/// <summary>
/// Represents a Minecraft-formatted endpoint.
/// </summary>
/// <param name="host">The <see cref="string"/> host part of the address.</param>
/// <param name="port">The <see cref="ushort"/> port part of the address.</param>
public readonly struct Address(string host, ushort port)
{
    /// <summary>
    /// The <see cref="string"/> host part.
    /// </summary>
    public string Host => host;

    /// <summary>
    /// The <see cref="ushort"/> port part.
    /// </summary>
    public ushort Port => port;

    /// <summary>
    /// Converts the <see cref="string"/> representation of an <see cref="Address"/> to the equivalent <see cref="Address"/> structure.
    /// </summary>
    /// <param name="input">A <see cref="ReadOnlySpan{T}"/> containing the UTF-8 characters representing the <see cref="Address"/> to convert.</param>
    /// <param name="address">Contains the converted <see cref="Address"/> equivalent</param>
    /// <returns><c>true</c> if <paramref name="input"/> was converted successfully; otherwise, <c>false</c>.</returns>
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