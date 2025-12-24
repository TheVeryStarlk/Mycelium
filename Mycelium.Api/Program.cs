using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Mycelium;
using Mycelium.Api;
using Mycelium.Bedrock;
using Mycelium.Java;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.TypeInfoResolverChain.Insert(0, ApplicationJsonSerializerContext.Default));

builder.Services.AddMycelium();
builder.Services.AddMemoryCache();

builder.Services.AddProblemDetails(options => options.CustomizeProblemDetails = details =>
{
    if (details.Exception is not { } exception)
    {
        return;
    }

    details.ProblemDetails = exception switch
    {
        MyceliumException => new ProblemDetails
        {
            Detail = exception.Message,
            Status = StatusCodes.Status400BadRequest
        },
        _ => new ProblemDetails
        {
            Detail = "An error has occurred.",
            Status = StatusCodes.Status500InternalServerError
        }
    };
});

var application = builder.Build();

application.UseExceptionHandler();

application.MapBedrock();
application.MapJava();

application.Run();

[JsonSerializable(typeof(BedrockResponse))]
[JsonSerializable(typeof(JavaResponse))]
internal sealed partial class ApplicationJsonSerializerContext : JsonSerializerContext;