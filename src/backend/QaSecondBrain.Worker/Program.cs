using QaSecondBrain.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddHostedService(sp => new HeartbeatService(
    sp.GetRequiredService<ILogger<HeartbeatService>>(),
    sp.GetRequiredService<TimeProvider>()));

var host = builder.Build();
await host.RunAsync();
