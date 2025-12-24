using Mycelium;
using Mycelium.Bedrock;
using Mycelium.Java;

var builder = WebApplication.CreateSlimBuilder(args);

// builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.TypeInfoResolverChain.Insert(0, ApplicationJsonSerializerContext.Default));

builder.Services.AddMycelium();

var application = builder.Build();

application.MapGet("/java", async (JavaClient client) => (await client.RequestStatusAsync("mc.hypixel.net:25565"))?.Description);
application.MapGet("/bedrock", async (BedrockClient client) => (await client.RequestStatusAsync("mco.cubecraft.net:19132"))?.Description.First());

application.Run();