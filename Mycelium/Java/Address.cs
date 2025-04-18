﻿namespace Mycelium.Java;

/// <summary>
/// Represents a Minecraft address.
/// </summary>
/// <param name="first">The <see cref="string"/> part of the address.</param>
/// <param name="port">The port part of the address.</param>
/// <example>mc.hypixel.net:25565</example>
internal readonly ref struct Address(ReadOnlySpan<char> first, ushort port)
{
    /// <summary>
    /// The <see cref="string"/> part of the address.
    /// </summary>
    public ReadOnlySpan<char> First { get; } = first;

    /// <summary>
    /// The port part of the address.
    /// </summary>
    public ushort Port { get; } = port;

    /// <summary>
    /// Tries to convert a <see cref="string"/> to an <see cref="Address"/>.
    /// </summary>
    /// <param name="input">The <see cref="string"/> to convert.</param>
    /// <param name="address">The <see cref="Address"/> resulting address.</param>
    /// <returns>True if the <see cref="string"/> was converted successfully, otherwise, false.</returns>
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