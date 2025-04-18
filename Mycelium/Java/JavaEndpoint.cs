namespace Mycelium.Java;

internal static class JavaEndpoint
{
    public static void AddJava(this IServiceCollection services)
    {
        services.AddTransient<JavaClient>();
    }

    public static void MapJava(this WebApplication application)
    {
        var group = application.MapGroup("/java");

        group.MapGet("/status/{address}", (string address) => "Hello, world!");
        group.MapGet("/ping/{address}", (string address) => "Hello, world!");
    }
}