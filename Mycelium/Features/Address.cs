namespace Mycelium.Features;

/// <summary>
/// Represents a Minecraft address.
/// </summary>
/// <param name="host">The <see cref="string"/> part of the address.</param>
/// <param name="port">The port part of the address.</param>
/// <example>mc.hypixel.net:25565</example>
internal readonly struct Address(string host, ushort port)
{
    /// <summary>
    /// The <see cref="string"/> part of the address.
    /// </summary>
    public string Host => host;

    /// <summary>
    /// The port part of the address.
    /// </summary>
    public ushort Port => port;

    /// <summary>
    /// Tries to convert a <see cref="ReadOnlySpan{T}"/> to an <see cref="Address"/>.
    /// </summary>
    /// <param name="input">The <see cref="ReadOnlySpan{T}"/> to convert.</param>
    /// <param name="address">The result <see cref="Address"/>.</param>
    /// <returns>True if the <see cref="ReadOnlySpan{T}"/> was converted successfully, otherwise, false.</returns>
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