WebApplicationBuilder builder = WebApplication.CreateSlimBuilder(args);

WebApplication app = builder.Build();

await app.RunAsync();