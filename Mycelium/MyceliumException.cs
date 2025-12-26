namespace Mycelium;

/// <summary>
/// An exception thrown by Mycelium.
/// </summary>
public sealed class MyceliumException : Exception
{
    internal MyceliumException(string message) : base(message)
    {
        // ...
    }
}