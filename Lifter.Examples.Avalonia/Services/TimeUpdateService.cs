using Microsoft.Extensions.Hosting;

namespace Lifter.Examples.Avalonia.Services;

public class TimeUpdateService : BackgroundService
{
    private readonly SharedStateService _sharedState;

    public TimeUpdateService(SharedStateService sharedState)
    {
        _sharedState = sharedState;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _sharedState.UpdateMessage($"Last update from Lifter service: {DateTime.Now:HH:mm:ss}");
                await timer.WaitForNextTickAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}
