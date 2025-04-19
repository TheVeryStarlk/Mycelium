using Mycelium;
using Mycelium.Features.Bedrock;
using Mycelium.Features.Java;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.TypeInfoResolverChain.Insert(0, MyceliumJsonSerializerContext.Default));

builder.Services.AddMemoryCache(options => options.ExpirationScanFrequency = TimeSpan.FromSeconds(500));
builder.Services.AddProblemDetails();

builder.Services.AddJava();
builder.Services.AddBedrock();

var application = builder.Build();

application.UseExceptionHandler();
application.UseStatusCodePages();
application.UseHttpsRedirection();

application.UseMiddleware<OperationCancelledMiddleware>();

application.MapJava();
application.MapBedrock();

application.Run();