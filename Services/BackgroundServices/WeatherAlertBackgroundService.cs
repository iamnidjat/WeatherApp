using WeatherApp.Services.BackgroundServices;
using WeatherApp.Services.Interfaces;

public class WeatherAlertBackgroundService : BackgroundService
{
    private readonly IServiceProvider _services;

    public WeatherAlertBackgroundService(IServiceProvider services)
    {
        _services = services;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _services.CreateScope())
                {
                    var worker = scope.ServiceProvider.GetRequiredService<WeatherAlertWorker>();
                    await worker.DoWork(stoppingToken);
                }

                // Wait for a specified interval before checking again
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // Check every hour
            }
            catch (OperationCanceledException)
            {
                // Gracefully handle cancellation
            }
            catch (Exception ex)
            {
                // Log any unexpected exceptions
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}

