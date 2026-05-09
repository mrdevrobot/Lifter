 # Lifter.Blazor

**Advanced `IHostedService` management and cross-platform dialog support for Blazor WebAssembly applications.**

---

## 📖 Overview

**Lifter.Blazor** brings the power and flexibility of the `IHostedService` pattern, common in ASP.NET Core backend applications, to the client-side world of Blazor WASM. Built on `Lifter.Core`, it provides a "WatchDog" to monitor, start, stop, and automatically restart your background tasks, managing their lifecycle robustly within a single-page application.

This library is ideal for periodic tasks like polling data from APIs, real-time state synchronization, or any other long-running logic that needs to live as long as the application is open in the browser.

## ✨ Features

* **Lifecycle Management**: Automatically starts and stops services along with the Blazor application.
* **Configurable Policies**: Define startup (`Automatic`/`Manual`) and restart (`OnFailure`) policies for each service.
* **Control API**: An `IHostManagerWatchDog` interface to monitor status and manually control services from any Razor component.
* **`IDialogService`**: Browser-native dialogs via JS interop (`alert` / `confirm`) with virtual override points for custom UI.
* **Simple Integration**: Set it up in two easy steps.

---

## 🚀 Quick Start Guide

Follow these steps to integrate Lifter into your Blazor WASM application.

### 1. Create Your Service

First, create your background service by inheriting from `BackgroundService` (the recommended base class for `IHostedService`).

```csharp
// Services/DataPollingService.cs
using Microsoft.Extensions.Hosting;

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
                        
                await timer.WaitForNextTick(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Expected behavior during shutdown
                break;
            }
        }
    }
}
```

### 2. Configure Services in `Program.cs`

Register Lifter and your own services in the Dependency Injection container.

```csharp
// Program.cs
using Lifter.Blazor;
using Lifter.Core;
using Lifter.Core.WatchDog;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// ... other service registrations

// 1. Add the Lifter framework
builder.Services.AddLifter();

// 2. Register your services with the desired policies
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


await builder.Build().RunAsync();
```

### 3. Add the Host to Your Main Layout

Finally, place the `<LifterHost />` component into your `MainLayout.razor` file (or another persistent layout). This invisible component will hook into the UI's lifecycle to start and stop your services.

```razor
@* MainLayout.razor *@
@inherits LayoutComponentBase
@using Lifter.Blazor

<div class="page">
    @* ... your sidebar markup ... *@

    <main>
        @* ... your top-row markup ... *@

        <article class="content px-4">
            @Body
        </article>
    </main>
</div>

@* Add this component, preferably at the end *@
<LifterHost />
```

**That's it!** On the next launch, `DataPollingService` will start automatically.

---

## 🛠️ Advanced Usage

### Monitoring and Controlling Services

You can inject `IHostManagerWatchDog` into any component to get the status of your services or to start/stop them manually.

```razor
@page "/dashboard"
@inject IHostManagerWatchDog WatchDog

<h3>Services Dashboard</h3>

@if (allStatuses is not null)
{
    <ul>
        @foreach (var status in allStatuses.Values)
        {
            <li>
                <strong>@status.Instance.GetType().Name</strong>: @status.Status
                @if (status.Status == ServiceStatus.Stopped)
                {
                    <button @onclick="() => StartService(status.Instance.GetType())">Start</button>
                }
            </li>
        }
    </ul>
}

@code {
    private IReadOnlyDictionary<Type, HostedServiceState>? allStatuses;

    protected override void OnInitialized()
    {
        allStatuses = WatchDog.GetAllStatuses();
        WatchDog.StatusChanged += (state) => 
        {
            allStatuses = WatchDog.GetAllStatuses();
            InvokeAsync(StateHasChanged);
        };
    }

    private async Task StartService(Type serviceType)
    {
        await WatchDog.StartServiceAsync(serviceType);
    }
}
```

---

## 🗨️ IDialogService

`BlazorDialogService` uses `IJSRuntime` to invoke the browser's native `alert` and `confirm` dialogs.

### Registration

```csharp
// Program.cs
builder.Services.AddBlazorDialogService();
```

### Usage in a Razor component

```razor
@inject IDialogService DialogService

<button @onclick="DeleteAsync">Delete</button>

@code {
    private async Task DeleteAsync()
    {
        var result = await DialogService.ShowConfirmAsync(
            "Delete",
            "Are you sure you want to delete this item?",
            new DialogOptions { ButtonSet = DialogButtonSet.YesNo });

        if (result.Confirmed)
        {
            // delete
        }
    }
}
```

> **Note:** `BootstrapBlazor` (and similar libraries) define their own `DialogResult` type. If you use both, add a using alias in your razor file:
> ```razor
> @using LifterDialogResult = Lifter.Core.Dialog.DialogResult
> ```

### Custom Override

```csharp
public class MyDialogService : BlazorDialogService
{
    public MyDialogService(IJSRuntime js) : base(js) { }

    // Replace browser alert/confirm with a custom modal library
    public override Task<DialogResult> ShowAsync(
        string title, object? content, DialogOptions? options, CancellationToken ct)
    {
        // open your custom modal and return the result
    }
}

builder.Services.AddBlazorDialogService<MyDialogService>();
```

---

## ⚠️ Important Limitations

The Blazor WebAssembly environment is fundamentally different from a server backend. It is crucial to understand the following limitation:

**The lifecycle is tied to the browser tab.** Lifter does its best to stop services gracefully when a user navigates away or closes the app. However, if the browser tab or the browser itself is **closed abruptly**, there is no guarantee that the `StopAsync` or `DisposeAsync` code will execute. Design your services to be resilient to a sudden termination.
        