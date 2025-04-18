var builder = WebApplication.CreateBuilder(args);

var application = builder.Build();

application.UseHttpsRedirection();

application.Run();