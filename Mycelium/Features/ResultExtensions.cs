using LightResults;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Mycelium.Features;

/// <summary>
/// Provides extensions for <see cref="Result"/>.
/// </summary>
internal static class ResultExtensions
{
    /// <summary>
    /// Creates a <see cref="TypedResults"/> from a <see cref="Result"/>.
    /// </summary>
    /// <param name="result">The <see cref="Result"/> to create from.</param>
    /// <typeparam name="T">The <see cref="T"/> type.</typeparam>
    /// <returns>A <see cref="TypedResults"/> created from <see cref="Result"/>.</returns>
    public static Results<Ok<T>, ProblemHttpResult> ToTypedResults<T>(this Result<T> result)
    {
        return result.IsSuccess(out var value)
            ? TypedResults.Ok(value)
            : TypedResults.Problem(statusCode: StatusCodes.Status400BadRequest, detail: result.Errors.First().Message);
    }
}