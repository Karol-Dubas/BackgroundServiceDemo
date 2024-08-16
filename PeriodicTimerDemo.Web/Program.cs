using PeriodicTimerDemo.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(o => o.SingleLine = true);

//builder.Services.AddHostedService<StandardRepeatingService>();
builder.Services.AddHostedService<PeriodicService>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();