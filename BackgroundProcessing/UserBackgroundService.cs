using System.Text.Json;
using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;

namespace BackgroundProcessing;

public class UserBackgroundService : BackgroundService
{
    private readonly Channel<int> _channel;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IServiceProvider _provider;
    private readonly ILogger<UserBackgroundService> _logger;
    private readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };

    public UserBackgroundService(Channel<int> channel,
        IHttpClientFactory httpClientFactory,
        IServiceProvider provider, 
        ILogger<UserBackgroundService> logger)
    {
        _channel = channel;
        _httpClientFactory = httpClientFactory;
        _provider = provider;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested && !_channel.Reader.Completion.IsCompleted)
        {
            try
            {
                int userId = await _channel.Reader.ReadAsync(ct);

                using var scope = _provider.CreateScope();
                var database = scope.ServiceProvider.GetRequiredService<Database>();

                if (await database.Users.AnyAsync(u => u.Id == userId, ct))
                    continue;

                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync($"https://jsonplaceholder.typicode.com/users/{userId}", ct);
                string content = await response.Content.ReadAsStringAsync(ct);
                var user = JsonSerializer.Deserialize<User>(content, _options);

                if (user == null)
                    continue;

                database.Users.Add(user);
                await database.SaveChangesAsync(ct);
                _logger.LogInformation("Used with id {Id} added", userId);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Operation canceled");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Adding user failed");
            }
        }
    }
}