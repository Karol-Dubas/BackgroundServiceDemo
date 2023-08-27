using PeriodicTimerDemo.Web;

var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddHostedService<RepeatingService>();
builder.Services.AddHostedService<PeriodicService>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();