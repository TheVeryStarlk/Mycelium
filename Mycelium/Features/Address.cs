namespace Mycelium.Features;

/// <summary>
/// Represents a Minecraft address.
/// </summary>
/// <param name="first">The <see cref="string"/> part of the address.</param>
/// <param name="port">The port part of the address.</param>
/// <example>mc.hypixel.net:25565</example>
internal readonly struct Address(string first, ushort port)
{
    /// <summary>
    /// The <see cref="string"/> part of the address.
    /// </summary>
    public string First => first;

    /// <summary>
    /// The port part of the address.
    /// </summary>
    public ushort Port => port;

    /// <summary>
    /// Tries to convert a <see cref="string"/> to an <see cref="Address"/>.
    /// </summary>
    /// <param name="input">The <see cref="string"/> to convert.</param>
    /// <param name="address">The result <see cref="Address"/>.</param>
    /// <returns>True if the <see cref="string"/> was converted successfully, otherwise, false.</returns>
    public static bool TryParse(ReadOnlySpan<char> input, out Address address)
    {
        // Make port optional?
        const char separator = ':';

        address = default;

        if (input.Count(separator) > 1)
        {
            return false;
        }

        var index = input.IndexOf(separator) + 1;

        if (index >= input.Length || !ushort.TryParse(input[index..], out var port) || index - 1 < 0)
        {
            return false;
        }

        var slice = input[..(index - 1)];

        if (slice.Length < 1)
        {
            return false;
        }

        address = new Address(slice.ToString(), port);

        return true;
    }
}