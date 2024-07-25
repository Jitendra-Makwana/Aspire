var builder = DistributedApplication.CreateBuilder(args);
// var cache = builder.AddRedis("cache");
builder.AddProject<Projects.AspireTest>("test");
    // .WithReference(cache);

builder.Build().Run();
