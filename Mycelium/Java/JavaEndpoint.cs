using Microsoft.AspNetCore.Http.HttpResults;

namespace Mycelium.Java;

internal static class JavaEndpoint
{
    public static void AddJava(this IServiceCollection services)
    {
        services.AddSingleton<JavaClient>();
    }

    public static void MapJava(this WebApplication application)
    {
        var group = application.MapGroup("/java");

        group.MapGet(
            "/status/{address}",
            async Task<Results<Ok<StatusResponse>, ProblemHttpResult>> (string address, JavaClient client) =>
            {
                var result = await client.RequestStatusAsync(address);
                return result.ToTypedResults();
            });
    }
}