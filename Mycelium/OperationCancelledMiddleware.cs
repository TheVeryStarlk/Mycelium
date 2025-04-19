namespace Mycelium;

internal sealed class OperationCancelledMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException)
        {
            context.Response.StatusCode = 409;
        }
    }
}