using Lifter.Blazor;
using Lifter.Core;
using Lifter.Core.WatchDog;
using Lifter.Examples.Blazor;
using Lifter.Examples.Blazor.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddBootstrapBlazor();

builder.Services.AddLifter();
builder.Services.AddBlazorDialogService();

builder.Services.AddSingleton<SharedStateService>();

// Example of a service with automatic startup and restart on failure, with max 3 retries and 5 seconds delay between attempts
builder.Services.AddHostedServiceWithPolicies<DataPollingService>(options =>
{
    options.Restart = RestartPolicy.OnFailure;
    options.MaxRestartAttempts = 3;
    options.RestartDelay = TimeSpan.FromSeconds(5);
});

// Example of another service with manual startup
builder.Services.AddHostedServiceWithPolicies<ManualTaskService>(options =>
{
    options.Startup = StartupPolicy.Manual;
});

// Example of a service with automatic startup and restart on failure
builder.Services.AddHostedServiceWithPolicies<TimeUpdateService>(options =>
{
    options.Startup = StartupPolicy.Automatic;
    options.Restart = RestartPolicy.OnFailure; // Let's make it resilient
});

await builder.Build().RunAsync();

public class DataPollingService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(15));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Console.WriteLine($"Polling data at {DateTime.Now}...");
                // Insert your API call or data update logic here.

                await timer.WaitForNextTickAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Expected behavior during shutdown
                break;
            }
        }
    }
}

public class ManualTaskService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // This service does nothing until started manually via WatchDog.
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("ManualTaskService started manually.");
        // Insert your manual startup logic here.
        await base.StartAsync(cancellationToken);
    }
}