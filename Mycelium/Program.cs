using Mycelium;
using Mycelium.Java;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.TypeInfoResolverChain.Insert(0, MyceliumJsonSerializerContext.Default));

builder.Services.AddMemoryCache(options => options.ExpirationScanFrequency = TimeSpan.FromSeconds(500));
builder.Services.AddProblemDetails();

builder.Services.AddJava();

var application = builder.Build();

application.UseExceptionHandler();
application.UseStatusCodePages();
application.UseHttpsRedirection();

application.MapJava();

application.Run();