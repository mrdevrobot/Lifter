---
layout: default
title: Installation
nav_order: 3
description: "Installation guide for Lifter packages"
permalink: /v2.0/installation
---

# Installation
{: .no_toc }

## Table of contents
{: .no_toc .text-delta }

1. TOC
{:toc}

---

## Prerequisites

All Lifter packages require:
- **.NET 10.0**
- Your chosen UI framework (MAUI, Avalonia 12, or Blazor)

---

## Package Installation

### Lifter.Core

The core library is automatically installed as a dependency when you install platform-specific packages. You typically don't need to install it directly.

```bash
dotnet add package Lifter.Core
```

**Use when:**
- Creating custom integrations
- Using only the HostManager/WatchDog without a UI framework

---

### Lifter.Maui

For .NET MAUI applications (iOS, Android, Windows, macOS).

```bash
dotnet add package Lifter.Maui
```

**Requirements:**
- .NET MAUI workload installed
- Target frameworks: `net10.0-*`

**Project file:**
```xml
<TargetFrameworks>net10.0-android;net10.0-ios;net10.0-maccatalyst</TargetFrameworks>
<UseMaui>true</UseMaui>
```

---

### Lifter.Avalonia

For Avalonia UI desktop and mobile applications.

```bash
dotnet add package Lifter.Avalonia
```

**Requirements:**
- Avalonia 12.0 or higher
- Target frameworks: `net10.0`

**Project file:**
```xml
<TargetFramework>net10.0</TargetFramework>
<PackageReference Include="Avalonia" Version="12.0.*" />
```

---

### Lifter.Blazor

For Blazor WebAssembly applications.

```bash
dotnet add package Lifter.Blazor
```

**Requirements:**
- Blazor WebAssembly project
- Target framework: `net10.0`

**Project file:**
```xml
<TargetFramework>net10.0</TargetFramework>
<PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly" Version="10.0.*" />
```

---

## Version Compatibility

| Lifter Version | .NET Version | MAUI | Avalonia | Blazor |
|:---------------|:-------------|:-----|:---------|:-------|
| 1.1.x          | 8.0, 9.0     | ✅   | ✅       | ✅     |
| 1.0.x          | 8.0, 9.0     | ✅   | ❌       | ✅     |

---

## Platform-Specific Setup

### .NET MAUI

**1. Install package:**
```bash
dotnet add package Lifter.Maui
```

**2. Update MauiProgram.cs:**
```csharp
using Lifter.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        // Register services and hosted services
        builder.Services.AddHostedService<MyBackgroundService>();

        // Enable Lifter
        builder.SupportHostedServices();

        return builder.Build();
    }
}
```

---

### Avalonia UI

**1. Install package:**
```bash
dotnet add package Lifter.Avalonia
```

**2. Create your App class:**
```csharp
using Lifter.Avalonia;

public class App : HostedApplication<MainView>
{
    protected override void ConfigureServices(
        IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddHostedService<MyBackgroundService>();
    }
}
```

**3. Update App.axaml:**
```xml
<avalonia:HostedApplication 
    xmlns="https://github.com/avaloniaui"
    xmlns:avalonia="using:Lifter.Avalonia"
    x:Class="MyApp.App">
</avalonia:HostedApplication>
```

---

### Blazor WebAssembly

**1. Install package:**
```bash
dotnet add package Lifter.Blazor
```

**2. Update Program.cs:**
```csharp
using Lifter.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddLifter();
builder.Services.AddHostedService<MyBackgroundService>();

await builder.Build().RunAsync();
```

**3. Add component to MainLayout.razor:**
```razor
@using Lifter.Blazor

<LifterHost />
```

---

## Advanced Installation

### With WatchDog Service

For advanced lifecycle control, add the WatchDog service:

```csharp
using Lifter.Core;
using Lifter.Core.WatchDog;

// Add WatchDog
builder.Services.AddLifterWatchDog();

// Register services with policies
builder.Services.AddHostedServiceWithPolicies<MyService>(options =>
{
    options.Startup = StartupPolicy.Automatic;
    options.Restart = RestartPolicy.OnFailure;
    options.MaxRestartAttempts = 5;
});
```

---

## Troubleshooting

### Common Issues

**Package restore fails:**
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages
dotnet restore
```

**Version conflicts:**
- Ensure all Lifter packages use the same version
- Check that .NET SDK version matches target framework

**MAUI workload missing:**
```bash
dotnet workload install maui
```

**Avalonia templates missing:**
```bash
dotnet new install Avalonia.Templates
```

---

## Next Steps

- 📖 [Read platform-specific guides](/Lifter/v2.0/packages/core)
- ⚙️ [Configure the WatchDog](/Lifter/v2.0/advanced/watchdog)
- 💡 [See examples](/Lifter/v2.0/examples/maui-webapp)
