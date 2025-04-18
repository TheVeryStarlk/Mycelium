using LightResults;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.Caching.Memory;

namespace Mycelium.Java;

internal sealed class JavaClient(ILogger<JavaClient> logger, IMemoryCache cache)
{
    private readonly SocketConnectionContextFactory factory = new(new SocketConnectionFactoryOptions(), logger);

    public async Task<Result<StatusResponse>> RequestStatusAsync(string input)
    {
        if (!Address.TryParse(input, out var address))
        {
            return Result.Failure<StatusResponse>("Invalid address.");
        }

        await Task.Delay(5);

        return Result.Success<StatusResponse>(StatusResponse.Create([]));
    }
}