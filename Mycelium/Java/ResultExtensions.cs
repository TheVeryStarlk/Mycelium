using LightResults;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Mycelium.Java;

/// <summary>
/// Provides helpful extensions for <see cref="Result"/>.
/// </summary>
internal static class ResultExtensions
{
    /// <summary>
    /// Converts the given <see cref="Result"/> to a <see cref="TypedResults"/>.
    /// </summary>
    /// <param name="result">The <see cref="Result"/> to convert.</param>
    /// <typeparam name="T">The <see cref="T"/> type.</typeparam>
    /// <returns>A <see cref="TypedResults"/> converted from <see cref="Result"/>.</returns>
    public static Results<Ok<T>, ProblemHttpResult> ToTypedResults<T>(this Result<T> result)
    {
        // There will be only one error message.
        return result.IsSuccess(out var value)
            ? TypedResults.Ok(value)
            : TypedResults.Problem(statusCode: StatusCodes.Status400BadRequest, detail: result.Errors.First().Message);
    }
}