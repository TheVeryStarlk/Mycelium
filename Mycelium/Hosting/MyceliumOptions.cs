namespace Mycelium.Hosting;

public sealed class MyceliumOptions
{
    public const string Section = "Mycelium";
    
    public TimeSpan ConnectionTimeout { get; set; } = TimeSpan.FromSeconds(30);

    public TimeSpan ReadingTimeout { get; set; } = TimeSpan.FromSeconds(15);

    public TimeSpan CacheLifetime { get; set; } = TimeSpan.FromMinutes(5);
}