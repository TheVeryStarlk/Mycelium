using System.Net;
using System.Net.Sockets;
using LightResults;

namespace Mycelium.Features;

/// <summary>
/// Provides host resolving methods with <see cref="Result"/>s.
/// </summary>
internal sealed class HostService(ILogger<HostService> logger)
{
    /// <summary>
    /// Resolves DNS host of a given address.
    /// </summary>
    /// <param name="address">The address to resolve DNS from.</param>
    /// <param name="token">A <see cref="CancellationToken"/> that can be used to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="Result"/> containing the DNS resolved <see cref="IPAddress"/>.</returns>
    public async Task<Result<IPAddress>> ResolveHostAsync(string address, CancellationToken token)
    {
        try
        {
            var addresses = await Dns.GetHostAddressesAsync(address, token);

            // Could try to take all resolved addresses and loop into each one till a valid connection is made.
            return addresses.Length is 0 ? Result.Failure<IPAddress>("Failed to resolve hostname.") : addresses[0];
        }
        catch (SocketException exception)
        {
            logger.LogDebug(exception, "Failed to resolve hostname");
            return Result.Failure<IPAddress>("Failed to resolve hostname.");
        }
    }
}