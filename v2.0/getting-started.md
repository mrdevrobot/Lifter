---
layout: default
title: Getting Started
nav_order: 2
description: "Get started with Lifter"
permalink: /v2.0/getting-started
---

# Getting Started
{: .no_toc }

## Table of contents
{: .no_toc .text-delta }

1. TOC
{:toc}

---

## Overview

Lifter is an ecosystem of .NET libraries that brings **`IHostedService`** support to UI frameworks. Run background tasks, local servers, and long-running services in your client applications with the same patterns you use on the server.

##  Choose Your Platform

| Platform | Package | Use Case |
|:---------|:--------|:---------|
| .NET MAUI | [Lifter.Maui](/Lifter/v2.0/packages/maui) | Mobile and desktop apps (iOS, Android, Windows, macOS) |
| Avalonia UI | [Lifter.Avalonia](/Lifter/v2.0/packages/avalonia) | Cross-platform desktop apps with HostedApplication |
| Blazor WASM | [Lifter.Blazor](/Lifter/v2.0/packages/blazor) | Browser-based applications |

---

## Quick Start

### .NET MAUI

```bash
dotnet add package Lifter.Maui
```

```csharp
// MauiProgram.cs
using Lifter.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        // Register your background service
        builder.Services.AddHostedService<MyBackgroundService>();

        // Enable Lifter support
        builder.SupportHostedServices();

        return builder.Build();
    }
}
```

### Avalonia UI

```bash
dotnet add package Lifter.Avalonia
```

```csharp
// App.cs
using Lifter.Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

public class App : HostedApplication<MainView>
{
    protected override void ConfigureServices(
        IServiceCollection services, 
        IConfiguration configuration)
    {
        // Configure window
        services.ConfigureWindow(config =>
        {
            config.Title = "My App";
            config.Width = 1024;
            config.Height = 768;
        });

        // Register services
        services.AddSingleton<IMyService, MyService>();
        
        // Add background services
        services.AddHostedService<MyBackgroundService>();
    }
}
```

### Blazor WebAssembly

```bash
dotnet add package Lifter.Blazor
```

```csharp
// Program.cs
using Lifter.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddLifter();
builder.Services.AddHostedService<MyBackgroundService>();

await builder.Build().RunAsync();
```

```razor
@* MainLayout.razor *@
<LifterHost />
```

---

## Next Steps

- 📚 [Read platform-specific guides](/Lifter/v2.0/packages/core)
- ⚙️ [Learn about the WatchDog service](/Lifter/v2.0/advanced/watchdog)
- 💡 [Explore examples](/Lifter/v2.0/examples/maui-webapp)

---

## IDialogService — Cross-Platform Dialogs

Available on all platforms from **Lifter 2.0**. Register once, use anywhere.

### Registration

```csharp
// Avalonia — in ConfigureServices
services.AddAvaloniaDialogService();

// Blazor — in Program.cs
builder.Services.AddBlazorDialogService();

// MAUI — in MauiProgram.cs
builder.AddMauiDialogService();
```

### Basic Usage

```csharp
// Inject IDialogService into any class or component
public class MyViewModel(IDialogService dialogs)
{
    public async Task GreetAsync()
    {
        await dialogs.ShowMessageAsync("Hello", "Welcome to Lifter!");
    }

    public async Task ConfirmAsync()
    {
        var result = await dialogs.ShowConfirmAsync(
            "Delete",
            "Are you sure?",
            new DialogOptions { ButtonSet = DialogButtonSet.YesNo });

        if (result.Confirmed) { /* proceed */ }
    }
}
```

### Custom Implementations

```csharp
// Subclass the platform implementation and register it
services.AddAvaloniaDialogService<MyCustomDialogService>();
builder.Services.AddBlazorDialogService<MyCustomBlazorDialogService>();
builder.AddMauiDialogService<MyCustomMauiDialogService>();
```
