using Lifter.Core.Dialog;
using Microsoft.Extensions.DependencyInjection;

namespace Lifter.Avalonia;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to configure Avalonia-specific services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures desktop window settings for the application.
    /// </summary>
    public static IServiceCollection ConfigureWindow(
        this IServiceCollection services,
        Action<WindowConfiguration> configure)
    {
        var config = new WindowConfiguration();
        configure(config);
        services.AddSingleton(config);
        return services;
    }

    /// <summary>
    /// Registers the default <see cref="AvaloniaDialogService"/> as the <see cref="IDialogService"/>.
    /// </summary>
    public static IServiceCollection AddAvaloniaDialogService(this IServiceCollection services)
        => services.AddAvaloniaDialogService<AvaloniaDialogService>();

    /// <summary>
    /// Registers a custom <typeparamref name="TDialogService"/> as the <see cref="IDialogService"/>.
    /// Use this overload to provide your own subclass of <see cref="AvaloniaDialogService"/>.
    /// </summary>
    public static IServiceCollection AddAvaloniaDialogService<TDialogService>(
        this IServiceCollection services)
        where TDialogService : class, IDialogService
    {
        services.AddSingleton<IDialogService, TDialogService>();
        return services;
    }
}
