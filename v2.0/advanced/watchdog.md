---
layout: default
title: WatchDog Service
parent: Advanced
nav_order: 1
permalink: /v2.0/advanced/watchdog
---

# WatchDog Service
{: .no_toc }

Advanced service monitoring and lifecycle management.

## Table of contents
{: .no_toc .text-delta }

1. TOC
{:toc}

---

## Overview

The **WatchDog** service is an advanced component of Lifter.Core that provides:

- 🎯 **Startup Policies** - Control when services start (Automatic/Manual)
- 🔄 **Restart Policies** - Automatic recovery from failures
- 📊 **Status Monitoring** - Real-time service state tracking
- 🔔 **Event Notifications** - React to state changes instantly
- 🛡️ **Resilience** - Configurable retry limits and backoff
- 🧵 **Thread-Safe** - Concurrent access protection

---

## When to Use WatchDog

### Use Regular HostManager When:
- Services always run automatically
- You don't need manual control
- Simple start/stop behavior is sufficient

### Use WatchDog When:
- Services need manual start/stop (user-triggered)
- Automatic restart on failure is required
- Real-time status monitoring is needed
- Multiple services with different policies

---

## Setup

### 1. Register WatchDog

```csharp
using Lifter.Core;

// Add WatchDog service
builder.Services.AddLifterWatchDog();
```

### 2. Register Services with Policies

```csharp
using Lifter.Core.WatchDog;

// Automatic service with restart
builder.Services.AddHostedServiceWithPolicies<SyncService>(options =>
{
    options.Startup = StartupPolicy.Automatic;
    options.Restart = RestartPolicy.OnFailure;
    options.MaxRestartAttempts = 5;
});

// Manual service
builder.Services.AddHostedServiceWithPolicies<WebServerService>(options =>
{
    options.Startup = StartupPolicy.Manual;
    options.Restart = RestartPolicy.Manual;
});
```

---

## Controlling Services

### Inject IHostManagerWatchDog

```csharp
using Lifter.Core.WatchDog;

public class MyViewModel
{
    private readonly IHostManagerWatchDog _watchDog;

    public MyViewModel(IHostManagerWatchDog watchDog)
    {
        _watchDog = watchDog;
    }
}
```

### Start a Service

```csharp
await _watchDog.StartServiceAsync(typeof(WebServerService));
```

### Stop a Service

```csharp
await _watchDog.StopServiceAsync(typeof(WebServerService));
```

### Get Service Status

```csharp
var state = _watchDog.GetStatus(typeof(WebServerService));
Console.WriteLine($"Status: {state.Status}");

if (state.Status == ServiceStatus.Failed)
{
    Console.WriteLine($"Error: {state.Exception?.Message}");
    Console.WriteLine($"Restart attempts: {state.RestartAttempts}");
}
```

---

## Monitoring State Changes

### Subscribe to Events

The WatchDog fires a `StatusChanged` event whenever any service changes state.

```csharp
_watchDog.StatusChanged += OnServiceStateChanged;

private void OnServiceStateChanged(HostedServiceState state)
{
    Console.WriteLine($"{state.Instance.GetType().Name}: {state.Status}");
}
```

### Platform-Specific UI Updates

**MAUI:**
```csharp
_watchDog.StatusChanged += (state) =>
{
    MainThread.BeginInvokeOnMainThread(() =>
    {
        UpdateUI(state);
    });
};
```

**Blazor:**
```csharp
protected override void OnInitialized()
{
    _watchDog.StatusChanged += OnStateChanged;
}

private void OnStateChanged(HostedServiceState state)
{
    InvokeAsync(StateHasChanged);
}

public void Dispose()
{
    _watchDog.StatusChanged -= OnStateChanged;
}
```

**Avalonia:**
```csharp
_watchDog.StatusChanged += (state) =>
{
    Dispatcher.UIThread.Post(() =>
    {
        UpdateUI(state);
    });
};
```

---

## Service Status States

```csharp
public enum ServiceStatus
{
    Stopped,      // Not running
    Starting,     // StartAsync in progress
    Running,      // Running normally
    Stopping,     // StopAsync in progress
    Failed,       // Failed with exception
    Restarting    // Attempting automatic restart
}
```

### State Transitions

```
Stopped → Starting → Running
Running → Stopping → Stopped
Running → Failed → (Restarting → Starting) OR Stopped
```

---

## Policies

### Startup Policy

#### Automatic
```csharp
options.Startup = StartupPolicy.Automatic;
```
Service starts when HostManager initializes.

#### Manual
```csharp
options.Startup = StartupPolicy.Manual;
```
Service must be started explicitly via `StartServiceAsync`.

### Restart Policy

#### OnFailure
```csharp
options.Restart = RestartPolicy.OnFailure;
options.MaxRestartAttempts = 3;
```
Automatically restarts up to N times if service fails.

#### Manual
```csharp
options.Restart = RestartPolicy.Manual;
```
Service stays stopped after failure. Manual restart required.

---

## Complete Examples

### Example 1: Resilient Background Sync

```csharp
// Registration
builder.Services.AddHostedServiceWithPolicies<SyncService>(options =>
{
    options.Startup = StartupPolicy.Automatic;
    options.Restart = RestartPolicy.OnFailure;
    options.MaxRestartAttempts = 5;
});

// Service implementation
public class SyncService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SyncDataAsync();
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
            catch (Exception ex)
            {
                // WatchDog will detect failure and restart
                throw new InvalidOperationException("Sync failed", ex);
            }
        }
    }
}
```

### Example 2: User-Controlled Web Server

```csharp
// Registration
builder.Services.AddHostedServiceWithPolicies<WebServerService>(options =>
{
    options.Startup = StartupPolicy.Manual;
    options.Restart = RestartPolicy.Manual;
});

// ViewModel control
public class ServerViewModel : ObservableObject
{
    private readonly IHostManagerWatchDog _watchDog;

    [ObservableProperty]
    private bool _isServerRunning;

    [ObservableProperty]
    private string _statusMessage = "Server stopped";

    public ServerViewModel(IHostManagerWatchDog watchDog)
    {
        _watchDog = watchDog;
        _watchDog.StatusChanged += OnServerStateChanged;
    }

    [RelayCommand]
    public async Task ToggleServerAsync()
    {
        if (IsServerRunning)
        {
            await _watchDog.StopServiceAsync(typeof(WebServerService));
        }
        else
        {
            await _watchDog.StartServiceAsync(typeof(WebServerService));
        }
    }

    private void OnServerStateChanged(HostedServiceState state)
    {
        if (state.Instance is WebServerService)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                IsServerRunning = state.Status == ServiceStatus.Running;
                StatusMessage = state.Status switch
                {
                    ServiceStatus.Running => "Server is running",
                    ServiceStatus.Starting => "Server is starting...",
                    ServiceStatus.Failed => $"Server failed: {state.Exception?.Message}",
                    _ => "Server stopped"
                };
            });
        }
    }
}
```

---

## Advanced Patterns

### Service Coordination

```csharp
// Start services in sequence
await _watchDog.StartServiceAsync(typeof(DatabaseService));
await Task.Delay(1000); // Wait for DB
await _watchDog.StartServiceAsync(typeof(ApiService));
```

### Health Monitoring

```csharp
public class HealthMonitor
{
    private readonly IHostManagerWatchDog _watchDog;

    public bool AreAllServicesHealthy()
    {
        var criticalServices = new[] 
        { 
            typeof(DatabaseService), 
            typeof(SyncService) 
        };

        return criticalServices.All(serviceType =>
        {
            var state = _watchDog.GetStatus(serviceType);
            return state.Status == ServiceStatus.Running;
        });
    }
}
```

---

## Best Practices

1. **Set appropriate MaxRestartAttempts** - Avoid infinite restart loops
2. **Handle StatusChanged on UI thread** - Platform-specific dispatching
3. **Clean up event subscriptions** - Prevent memory leaks
4. **Use timeout for long operations** - Don't block forever
5. **Log state transitions** - Helpful for debugging

---

## Troubleshooting

### Service won't restart

**Check:**
- `MaxRestartAttempts` not exceeded
- `RestartPolicy` is `OnFailure`
- Service throws exception (doesn't return gracefully)

### Memory leak

**Fix:**
```csharp
// Always unsubscribe in Dispose
public void Dispose()
{
    _watchDog.StatusChanged -= OnStateChanged;
}
```

### Race conditions

WatchDog is thread-safe, but ensure UI updates happen on the correct thread.

---

## Next Steps

- 📚 [Startup and Restart Policies](/Lifter/v2.0/advanced/policies)
- 🧵 [Threading Considerations](/Lifter/v2.0/advanced/threading)
- 💡 [See Examples](/Lifter/v2.0/examples/maui-webapp)
