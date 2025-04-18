var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.TypeInfoResolverChain.Insert(0, MyceliumJsonSerializerContext.Default));
builder.Services.AddMemoryCache(options => options.ExpirationScanFrequency = TimeSpan.FromMinutes(1));

var application = builder.Build();

application.UseHttpsRedirection();
application.Run();