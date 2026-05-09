# Lifter.Maui

[![NuGet](https://img.shields.io/nuget/v/Lifter.Maui.svg)](https://www.nuget.org/packages/Lifter.Maui/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

**.NET MAUI** integration for **Lifter** — brings `IHostedService` lifecycle management and the `IDialogService` abstraction to iOS, Android, macOS, and Windows apps.

---

## Features

- **IHostedService support** — Start and stop background services with the MAUI app lifecycle
- **WatchDog** — Monitor, restart, and control services at runtime
- **MauiDialogService** — Native MAUI dialogs via `Page.DisplayAlert`, fully virtual for override

---

## Quick Start

```bash
dotnet add package Lifter.Maui
```

### 1. Register services in `MauiProgram.cs`

```csharp
using Lifter.Core;
using Lifter.Core.WatchDog;
using Lifter.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        // Register background services with policies
        builder.Services.AddLifterWatchDog();
        builder.Services.AddHostedServiceWithPolicies<MyBackgroundService>(options =>
        {
            options.Startup = StartupPolicy.Automatic;
            options.Restart = RestartPolicy.OnFailure;
            options.MaxRestartAttempts = 3;
        });

        // Register MauiDialogService
        builder.AddMauiDialogService();

        // Wire Lifter into the MAUI lifecycle
        builder.SupportHostedServices();

        return builder.Build();
    }
}
```

### 2. Inject and use IDialogService

```csharp
using Lifter.Core.Dialog;

public partial class MainPage : ContentPage
{
    private readonly IDialogService _dialogs;

    public MainPage(IDialogService dialogs)
    {
        InitializeComponent();
        _dialogs = dialogs;
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var result = await _dialogs.ShowConfirmAsync(
            "Delete",
            "Are you sure?",
            new DialogOptions { ButtonSet = DialogButtonSet.YesNo });

        if (result.Confirmed)
        {
            // delete logic
        }
    }
}
```

---

## WatchDog — Control Services at Runtime

```csharp
public partial class SettingsPage : ContentPage
{
    private readonly IHostManagerWatchDog _watchDog;

    public SettingsPage(IHostManagerWatchDog watchDog)
    {
        InitializeComponent();
        _watchDog = watchDog;

        _watchDog.StatusChanged += state =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                StatusLabel.Text = $"{state.Instance.GetType().Name}: {state.Status}";
            });
        };
    }

    async void OnStartClicked(object s, EventArgs e) =>
        await _watchDog.StartServiceAsync(typeof(MyBackgroundService));

    async void OnStopClicked(object s, EventArgs e) =>
        await _watchDog.StopServiceAsync(typeof(MyBackgroundService));
}
```

---

## Custom Dialog Override

```csharp
public class MyDialogService : MauiDialogService
{
    // Override to use a custom page or modal
    protected override Page? GetMainPage() =>
        Application.Current?.MainPage;
}

builder.AddMauiDialogService<MyDialogService>();
```

---

## Documentation

Full documentation at **[lucafabbri.github.io/Lifter](https://lucafabbri.github.io/Lifter/latest/getting-started)**.

## Related packages

| Package | Description |
|:--------|:------------|
| [Lifter.Core](https://www.nuget.org/packages/Lifter.Core/) | Core abstractions — `HostManager`, `WatchDog`, `IDialogService` |
| [Lifter.Avalonia](https://www.nuget.org/packages/Lifter.Avalonia/) | Avalonia UI 12 integration |
| [Lifter.Blazor](https://www.nuget.org/packages/Lifter.Blazor/) | Blazor WebAssembly integration |
