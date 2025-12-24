using System.IO.Pipelines;
using System.Net.Sockets;
using System.Text;
using Mycelium.Java.Packets;

namespace Mycelium.Java;

public sealed class JavaClient(ISocketFactory factory)
{
    public JavaClient() : this(new SocketFactory())
    {
        // What about logging?
    }

    public async Task<string?> RequestStatusAsync(Address address, CancellationToken token = default)
    {
        var socket = await factory.ConnectAsync(address, SocketType.Dgram, ProtocolType.Udp, token);

        await using var stream = new NetworkStream(socket, true);
        
        var output = PipeWriter.Create(stream);
        var input = PipeReader.Create(stream);

        try
        {
            await StatusRequestPacket.WriteAsync(output, address.Host, address.Port, token);
            return Encoding.UTF8.GetString(await StatusResponsePacket.ReadAsync(input, token));
        }
        finally
        {
            await output.CompleteAsync();
            await input.CompleteAsync();
        }
    }
}