using LightResults;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Mycelium.Java;

internal static class ResultExtensions
{
    public static Results<Ok<T>, ProblemHttpResult> ToTypedResults<T>(this Result<T> result)
    {
        return result.IsSuccess(out var value)
            ? TypedResults.Ok(value)
            : TypedResults.Problem(statusCode: StatusCodes.Status400BadRequest, detail: result.Errors.First().Message);
    }
}