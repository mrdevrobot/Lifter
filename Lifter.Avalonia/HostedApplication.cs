using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Lifter.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Lifter.Avalonia;

/// <summary>
/// An abstract base class for creating a hosted Avalonia application with 
/// dependency injection, configuration, and IHostedService support.
/// </summary>
/// <typeparam name="TMainView">
/// The main view type. Must be a Control and implement <see cref="IHostedView"/>.
/// </typeparam>
public abstract class HostedApplication<TMainView> : global::Avalonia.Application
    where TMainView : Control, IHostedView
{
    /// <summary>
    /// The host application builder used to configure services and configuration.
    /// </summary>
    protected readonly HostApplicationBuilder _hostApplicationBuilder;

    /// <summary>
    /// Gets the application's configured services.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the application host has not been built yet.</exception>
    public IServiceProvider Services => Host?.Services
        ?? throw new InvalidOperationException("The application host has not been built yet.");

    /// <summary>
    /// Gets the application's configuration.
    /// </summary>
    public IConfiguration Configuration => _hostApplicationBuilder.Configuration;

    /// <summary>
    /// Gets the configured application host.
    /// </summary>
    public IHost? Host { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HostedApplication{TMainView}"/> class.
    /// </summary>
    protected HostedApplication()
    {
        _hostApplicationBuilder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder();
    }

    /// <inheritdoc/>
    public sealed override void OnFrameworkInitializationCompleted()
    {
        // Show splash screen if provided (optional)
        Window? splashWindow = null;
        var splashScreenContent = CreateSplashScreen();

        if (splashScreenContent != null)
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                splashWindow = new Window
                {
                    Content = splashScreenContent,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    WindowDecorations = WindowDecorations.None,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    Title = "Loading..."
                };
                desktop.MainWindow = splashWindow;
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
            {
                singleView.MainView = splashScreenContent;
            }
        }

        // Build the host
        Host = BuildHostInternal();

        // Start initialization on background thread
        _ = Task.Run(async () =>
        {
            try
            {
                // Allow derived classes to perform custom initialization
                await OnHostInitializedAsync();

                // Switch to UI thread for view setup
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    SetupMainView(splashWindow);
                });

                // Start hosted services
                HostManager.Initialize(Services);
                await HostManager.StartAsync();
            }
            catch (Exception ex)
            {
                // Log or handle startup errors
                Console.WriteLine($"Application startup failed: {ex}");
                throw;
            }
        });

        base.OnFrameworkInitializationCompleted();
    }

    /// <summary>
    /// Builds and configures the application host.
    /// </summary>
    private IHost BuildHostInternal()
    {
        // Register the main view as a singleton
        _hostApplicationBuilder.Services.AddSingleton<TMainView>();

        // Allow derived classes to register services
        ConfigureServices(_hostApplicationBuilder.Services, _hostApplicationBuilder.Configuration);

        return _hostApplicationBuilder.Build();
    }

    /// <summary>
    /// Sets up the main view for Desktop or Mobile platforms.
    /// </summary>
    /// <param name="splashWindow">The splash window to close after setup (Desktop only).</param>
    private void SetupMainView(Window? splashWindow)
    {
        var mainView = Services.GetRequiredService<TMainView>();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Desktop: Create a Window with the view as content
            var windowConfig = Services.GetService<WindowConfiguration>() ?? new WindowConfiguration();

            var mainWindow = new Window
            {
                Title = windowConfig.Title,
                Content = mainView,
                Width = windowConfig.Width,
                Height = windowConfig.Height,
                WindowState = windowConfig.WindowState,
                WindowStartupLocation = windowConfig.StartupLocation
            };

#if DEBUG
            // Enable DevTools in debug mode
            mainWindow.AttachDevTools();
#endif

            desktop.MainWindow = mainWindow;
            mainWindow.Show();

            // Close splash screen
            splashWindow?.Close();
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
        {
            // Mobile: Set the view directly
            singleView.MainView = mainView;
        }
    }

    /// <summary>
    /// Creates the splash screen control to be displayed during initialization.
    /// </summary>
    /// <returns>The splash screen control, or null if no splash screen should be shown.</returns>
    /// <remarks>
    /// Override this method in derived classes to provide a custom splash screen.
    /// The splash screen will be shown while the application is initializing.
    /// </remarks>
    protected virtual Control? CreateSplashScreen()
    {
        return null; // No splash screen by default
    }

    /// <summary>
    /// Called when the host has completed its initialization process asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// Override this method to perform custom initialization tasks after the host is built
    /// but before the main view is displayed. This runs on a background thread.
    /// </remarks>
    protected virtual Task OnHostInitializedAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Configures application services and registers them with the dependency injection container.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which services are added.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance containing application configuration settings.</param>
    /// <remarks>
    /// This method is called during application startup to configure services.
    /// Use this method to register your application's services, background services, and dependencies.
    /// </remarks>
    protected abstract void ConfigureServices(IServiceCollection services, IConfiguration configuration);
}
