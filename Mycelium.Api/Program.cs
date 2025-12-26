using System.Text.Json.Serialization;
using Mycelium;
using Mycelium.Api;
using Mycelium.Bedrock;
using Mycelium.Java;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.TypeInfoResolverChain.Insert(0, ApplicationJsonSerializerContext.Default));

builder.Services.AddMycelium();
builder.Services.AddProblemDetails();

var application = builder.Build();

application.UseExceptionHandler();

application.MapBedrock();
application.MapJava();

application.Run();

[JsonSerializable(typeof(BedrockResponse))]
[JsonSerializable(typeof(JavaResponse))]
internal sealed partial class ApplicationJsonSerializerContext : JsonSerializerContext;