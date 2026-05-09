using Avalonia.Themes.Fluent;
using Lifter.Avalonia;
using Lifter.Core;
using Lifter.Core.WatchDog;
using Lifter.Examples.Avalonia.Services;
using Lifter.Examples.Avalonia.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lifter.Examples.Avalonia;

public class App : HostedApplication<MainView>
{
    public override void Initialize()
    {
        Styles.Add(new FluentTheme());
    }

    protected override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddLifterWatchDog();
        services.AddAvaloniaDialogService();

        services.AddSingleton<SharedStateService>();

        services.AddHostedServiceWithPolicies<TimeUpdateService>(options =>
        {
            options.Startup = StartupPolicy.Automatic;
            options.Restart = RestartPolicy.OnFailure;
        });

        services.ConfigureWindow(config =>
        {
            config.Title = "Lifter Avalonia Example";
            config.Width = 800;
            config.Height = 560;
        });
    }
}
