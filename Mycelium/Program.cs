var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.TypeInfoResolverChain.Insert(0, MyceliumJsonSerializerContext.Default));
builder.Services.AddMemoryCache(options => options.ExpirationScanFrequency = TimeSpan.FromMinutes(1));
builder.Services.AddProblemDetails();

var application = builder.Build();

application.UseExceptionHandler();
application.UseStatusCodePages();

application.UseHttpsRedirection();
application.Run();