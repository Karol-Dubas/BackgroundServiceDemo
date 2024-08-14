using System.Threading.Channels;
using BackgroundProcessing;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<Database>(config =>
    config.UseInMemoryDatabase("database"));

builder.Services.AddHttpClient();
builder.Services.AddSingleton<Channel<int>>(_ => Channel.CreateUnbounded<int>());
builder.Services.AddHostedService<UserBackgroundService>();

var app = builder.Build();

app.MapGet("/items", (Database database) => database.Users.ToListAsync());

app.MapPost("/create/{id:int}", async (int id, Channel<int> channel) =>
{
    await channel.Writer.WriteAsync(id);
    return Results.Accepted();
});

app.Run();