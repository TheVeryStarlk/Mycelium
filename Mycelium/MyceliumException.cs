namespace Mycelium;

public sealed class MyceliumException : Exception
{
    internal MyceliumException(string message) : base(message)
    {
        // Empty...
    }
}