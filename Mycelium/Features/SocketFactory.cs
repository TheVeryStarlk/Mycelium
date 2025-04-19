﻿using System.Net;
using System.Net.Sockets;
using LightResults;

namespace Mycelium.Features;

/// <summary>
/// A factory for creating <see cref="Socket"/>.
/// </summary>
internal sealed class SocketFactory
{
    /// <summary>
    /// Creates a <see cref="Socket"/> to the given address and port.
    /// </summary>
    /// <param name="edition">The Minecraft edition used for creating the <see cref="Socket"/>.</param>
    /// <param name="address">The address to create the <see cref="Socket"/> to.</param>
    /// <param name="port">The port to create the <see cref="Socket"/> to.</param>
    /// <param name="token">A <see cref="CancellationToken"/> that can be used to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="Result"/> containing the UDP <see cref="Socket"/>.</returns>
    public async Task<Result<Socket>> ConnectAsync(Edition edition, string address, ushort port, CancellationToken token)
    {
        var resolving = await ResolveHostAsync(address, token);

        if (!resolving.IsSuccess(out var host))
        {
            return resolving.AsFailure<Socket>();
        }

        var socket = new Socket(
            host.AddressFamily,
            edition is Edition.Java ? SocketType.Stream : SocketType.Dgram,
            edition is Edition.Java ? ProtocolType.Tcp : ProtocolType.Udp);

        try
        {
            await socket.ConnectAsync(host, port, token);
        }
        catch (SocketException)
        {
            return Result.Failure<Socket>("Could not connect to the server.");
        }

        return socket;
    }

    /// <summary>
    /// Resolves DNS host of a given address.
    /// </summary>
    /// <param name="address">The address to resolve DNS from.</param>
    /// <param name="token">A <see cref="CancellationToken"/> that can be used to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="Result"/> containing the DNS resolved <see cref="IPAddress"/>.</returns>
    private static async Task<Result<IPAddress>> ResolveHostAsync(string address, CancellationToken token)
    {
        try
        {
            var addresses = await Dns.GetHostAddressesAsync(address, token);

            // Could try to take all resolved addresses and loop into each one till a valid connection is made.
            return addresses.Length is 0 ? Result.Failure<IPAddress>("Failed to resolve hostname.") : addresses[0];
        }
        catch (SocketException)
        {
            return Result.Failure<IPAddress>("Failed to resolve hostname.");
        }
    }
}