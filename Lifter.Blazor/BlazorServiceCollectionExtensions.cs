using Lifter.Core;
using Lifter.Core.Dialog;
using Microsoft.Extensions.DependencyInjection;

namespace Lifter.Blazor;

public static class BlazorServiceCollectionExtensions
{
    /// <summary>
    /// Registers the Lifter WatchDog and all necessary services
    /// for managing IHostedService instances within a Blazor WASM application.
    /// </summary>
    public static IServiceCollection AddLifter(this IServiceCollection services)
    {
        services.AddLifterWatchDog();
        return services;
    }

    /// <summary>
    /// Registers the default <see cref="BlazorDialogService"/> as the <see cref="IDialogService"/>.
    /// The implementation uses the browser's native <c>alert</c>/<c>confirm</c> APIs.
    /// </summary>
    public static IServiceCollection AddBlazorDialogService(this IServiceCollection services)
        => services.AddBlazorDialogService<BlazorDialogService>();

    /// <summary>
    /// Registers a custom <typeparamref name="TDialogService"/> as the <see cref="IDialogService"/>.
    /// Use this overload to provide your own subclass (e.g. backed by BootstrapBlazor or MudBlazor).
    /// </summary>
    public static IServiceCollection AddBlazorDialogService<TDialogService>(
        this IServiceCollection services)
        where TDialogService : class, IDialogService
    {
        services.AddScoped<IDialogService, TDialogService>();
        return services;
    }
}