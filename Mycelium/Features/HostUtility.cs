using System.Net;
using LightResults;

namespace Mycelium.Features;

/// <summary>
/// Provides <see cref="Dns"/> hostname resolving.
/// </summary>
internal static class HostUtility
{
    /// <summary>
    /// Resolves DNS host of a given address.
    /// </summary>
    /// <param name="address">The address to resolve DNS from.</param>
    /// <returns>A <see cref="Result"/> containing the DNS resolved <see cref="IPAddress"/>.</returns>
    public static async Task<Result<IPAddress>> ResolveHostAsync(string address)
    {
        try
        {
            var addresses = await Dns.GetHostAddressesAsync(address);

            // I think one is enough. We could try to take all resolved addresses and loop into each one till we get a valid connection.
            return addresses.Length is 0 ? Result.Failure<IPAddress>("Failed to resolve hostname.") : addresses[0];
        }
        catch
        {
            return Result.Failure<IPAddress>("An exception occured while trying to resolve hostname.");
        }
    }
}