using Lifter.Core;
using Lifter.Core.Dialog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.LifecycleEvents;

namespace Lifter.Maui;

/// <summary>
/// Provides the extension method to enable IHostedService support in a .NET MAUI application.
/// </summary>
public static class MauiAppBuilderExtensions
{
    /// <summary>
    /// Configures the application to automatically start and stop registered IHostedService instances
    /// based on the application's lifecycle events.
    /// </summary>
    /// <param name="builder">The MauiAppBuilder to configure.</param>
    /// <returns>The configured MauiAppBuilder.</returns>
    public static MauiAppBuilder SupportHostedServices(this MauiAppBuilder builder)
    {
        builder.ConfigureLifecycleEvents(events =>
        {
#if WINDOWS
            // Correction for CS1061: Use MauiWinUIApplication.Current.Services to get the service provider.
            events.AddWindows(windows => windows
                .OnLaunched((window, args) => StartHost(MauiWinUIApplication.Current.Services))
                .OnClosed((window, args) => StopHost()));
#elif ANDROID
            events.AddAndroid(android => android
                .OnCreate((activity, bundle) => StartHost(MauiApplication.Current.Services))
                .OnDestroy(activity => StopHost()));
#elif IOS || MACCATALYST
            // For both iOS and MacCatalyst, MAUI uses the AddiOS extension method 
            // as they share the same application lifecycle foundation.
            events.AddiOS(ios => ios
                .FinishedLaunching((app, options) => {
                    StartHost(MauiUIApplicationDelegate.Current.Services);
                    return true;
                })
                .WillTerminate(app => StopHost()));
#endif
        });

        return builder;
    }

    private static void StartHost(IServiceProvider services)
    {
        HostManager.Initialize(services);
        HostManager.StartAsync().GetAwaiter().GetResult();
    }



    private static void StopHost()
    {
        HostManager.StopAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Registers the default <see cref="MauiDialogService"/> as the <see cref="IDialogService"/>.
    /// </summary>
    public static MauiAppBuilder AddMauiDialogService(this MauiAppBuilder builder)
        => builder.AddMauiDialogService<MauiDialogService>();

    /// <summary>
    /// Registers a custom <typeparamref name="TDialogService"/> as the <see cref="IDialogService"/>.
    /// Use this overload to provide your own subclass of <see cref="MauiDialogService"/>.
    /// </summary>
    public static MauiAppBuilder AddMauiDialogService<TDialogService>(this MauiAppBuilder builder)
        where TDialogService : class, IDialogService
    {
        builder.Services.AddSingleton<IDialogService, TDialogService>();
        return builder;
    }
}
