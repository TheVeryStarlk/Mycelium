namespace Mycelium.Java;

public sealed class JavaClient(ISocketFactory factory)
{
    public JavaClient() : this(new SocketFactory())
    {
        // What about logging?
    }
}