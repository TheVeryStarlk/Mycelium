namespace Mycelium;

/// <summary>
/// A middleware for handling <see cref="OperationCanceledException"/>s in the application.
/// </summary>
internal sealed class OperationCancelledMiddleware(RequestDelegate next)
{
    /// <summary>
    /// Invokes a <see cref="RequestDelegate"/> and catches an <see cref="OperationCanceledException"/>.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current <see cref="RequestDelegate"/>.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous invoke operation.</returns>
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