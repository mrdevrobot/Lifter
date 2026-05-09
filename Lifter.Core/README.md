# Lifter.Core

[![NuGet](https://img.shields.io/nuget/v/Lifter.Core.svg)](https://www.nuget.org/packages/Lifter.Core/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Platform-agnostic core library for the **Lifter** ecosystem. Provides the `HostManager`, the advanced `WatchDog` service, and the `IDialogService` abstraction layer used by all Lifter platform packages.

> Typically installed automatically as a dependency of `Lifter.Maui`, `Lifter.Avalonia`, or `Lifter.Blazor`. Install directly only for custom integrations.

---

## Features

- **HostManager** — Manages `IHostedService` lifecycle (start / stop) tied to your app
- **WatchDog** — Advanced service monitoring with startup/restart policies and real-time status events
- **IDialogService** — Cross-platform dialog abstraction (`ShowMessageAsync`, `ShowConfirmAsync`, `ShowAsync`)
- **NullDialogService** — No-op implementation for testing and default registrations

---

## IDialogService

```csharp
using Lifter.Core.Dialog;

public class MyViewModel(IDialogService dialogs)
{
    public async Task DeleteAsync()
    {
        var result = await dialogs.ShowConfirmAsync(
            "Delete item",
            "Are you sure you want to delete this item?",
            new DialogOptions { ButtonSet = DialogButtonSet.YesNo });

        if (result.Confirmed)
        {
            // proceed
        }
    }
}
```

### DialogResult values

| Property | `Button` | `Confirmed` |
|:---------|:---------|:------------|
| `DialogResult.Ok` | `Ok` | `true` |
| `DialogResult.Yes` | `Yes` | `true` |
| `DialogResult.Cancel` | `Cancel` | `false` |
| `DialogResult.No` | `No` | `false` |
| `DialogResult.Close` | `Close` | `false` |
| `DialogResult.None` | `None` | `false` |

---

## WatchDog

```csharp
using Lifter.Core;
using Lifter.Core.WatchDog;

// Register
services.AddLifterWatchDog();
services.AddHostedServiceWithPolicies<MyService>(options =>
{
    options.Startup = StartupPolicy.Automatic;
    options.Restart = RestartPolicy.OnFailure;
    options.MaxRestartAttempts = 5;
});

// Control
public class MyViewModel(IHostManagerWatchDog watchDog)
{
    public Task StopAsync() => watchDog.StopServiceAsync(typeof(MyService));
    public HostedServiceState State => watchDog.GetStatus(typeof(MyService));
}
```

---

## Documentation

Full documentation at **[lucafabbri.github.io/Lifter](https://lucafabbri.github.io/Lifter/latest/getting-started)**.

## Platform packages

| Package | Description |
|:--------|:------------|
| [Lifter.Avalonia](https://www.nuget.org/packages/Lifter.Avalonia/) | Avalonia UI 12 — `HostedApplication` + `AvaloniaDialogService` |
| [Lifter.Blazor](https://www.nuget.org/packages/Lifter.Blazor/) | Blazor WebAssembly — `LifterHost` component + `BlazorDialogService` |
| [Lifter.Maui](https://www.nuget.org/packages/Lifter.Maui/) | .NET MAUI — lifecycle integration + `MauiDialogService` |
