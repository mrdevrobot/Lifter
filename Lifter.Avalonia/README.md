# Lifter.Avalonia

Avalonia UI 12 integration for Lifter, providing `HostedApplication` support with dependency injection, configuration, `IHostedService` lifecycle management, and the cross-platform `IDialogService`.

## Features

✅ **Dependency Injection** - Full `IServiceCollection` and `IServiceProvider` support  
✅ **Configuration** - `IConfiguration` with JSON, environment variables, and user secrets  
✅ **IHostedService Support** - Background services managed by application lifecycle  
✅ **IDialogService** - Native Avalonia dialog windows with full override points  
✅ **Cross-Platform** - Proper Desktop (Window) and Mobile (SingleView) handling  
✅ **Splash Screen** - Optional splash screen during initialization  
✅ **Customizable** - Configurable window settings and lifecycle hooks

## Installation

```bash
dotnet add package Lifter.Avalonia
```

## Quick Start

### 1. Create Your Main View

```csharp
using Avalonia.Controls;
using Lifter.Avalonia;

public partial class MainView : UserControl, IHostedView
{
    public MainView()
    {
        InitializeComponent();
    }
}
```

### 2. Create Your App Class

```csharp
using Lifter.Avalonia;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class App : HostedApplication<MainView>
{
    protected override void ConfigureServices(
        IServiceCollection services, 
        IConfiguration configuration)
    {
        // Configure window settings
        services.ConfigureWindow(config =>
        {
            config.Title = "My Avalonia App";
            config.Width = 1024;
            config.Height = 768;
        });

        // Register your services
        services.AddSingleton<IMyService, MyService>();
        
        // Register background services
        services.AddHostedService<MyBackgroundService>();
    }
}
```

### 3. Update App.axaml

```xml
<avalonia:HostedApplication 
    xmlns="https://github.com/avaloniaui"
    xmlns:avalonia="using:Lifter.Avalonia"
    x:Class="MyApp.App">
</avalonia:HostedApplication>
```

## Advanced Usage

### Custom Initialization

Override `OnHostInitializedAsync` for custom initialization logic:

```csharp
protected override async Task OnHostInitializedAsync()
{
    // Perform database migration, load cached data, etc.
    var dbContext = Services.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}
```

### Splash Screen

Override `CreateSplashScreen` to show a custom splash screen:

```csharp
protected override Control? CreateSplashScreen()
{
    return new SplashScreenView();
}
```

### Background Services

Implement `IHostedService` for long-running background tasks:

```csharp
public class DataSyncService : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Start background task
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Clean up
        return Task.CompletedTask;
    }
}

// Register in ConfigureServices
services.AddHostedService<DataSyncService>();
```

## Platform Support

- ✅ **Desktop** (Windows, macOS, Linux) - Creates a `Window` with your view
- ✅ **Mobile** (iOS, Android via .NET MAUI) - Direct view assignment
- ✅ **.NET 10** — targets `net10.0`
- ✅ **Avalonia 12.0**

## How It Works

### Desktop vs Mobile

**Desktop (`IClassicDesktopStyleApplicationLifetime`):**
```csharp
var mainWindow = new Window
{
    Content = mainView,  // Your view becomes window content
    Title = "App",
    Width = 1200,
    Height = 960
};
desktop.MainWindow = mainWindow;
mainWindow.Show();
```

**Mobile (`ISingleViewApplicationLifetime`):**
```csharp
singleView.MainView = mainView;  // Direct view assignment
```

### Initialization Sequence

1. **Build Host** - Create `IHost` with DI and configuration
2. **Show Splash** - Optional splash screen
3. **Async Init** - Run `OnHostInitializedAsync` on background thread
4. **Setup View** - Create Window (Desktop) or set MainView (Mobile) on UI thread
5. **Start Services** - Start all `IHostedService` instances

## Architecture

```
HostedApplication<TMainView>
    ├── HostApplicationBuilder (Microsoft.Extensions.Hosting)
    ├── IServiceProvider (Dependency Injection)
    ├── IConfiguration (Configuration)
    └── HostManager (Lifter.Core - manages IHostedService lifecycle)
```

## License

MIT - See LICENSE file for details

## Related Packages

- **Lifter.Core** - Core abstractions, `HostManager`, and `IDialogService` interfaces
- **Lifter.Maui** - .NET MAUI integration for Lifter
- **Lifter.Blazor** - Blazor WebAssembly integration for Lifter

## IDialogService

`AvaloniaDialogService` shows native Avalonia `Window` dialogs parented to the desktop main window.

### Registration

```csharp
// In ConfigureServices (App.cs)
services.AddAvaloniaDialogService();
```

### Usage

```csharp
public class MainView : UserControl, IHostedView
{
    private readonly IDialogService _dialogs;

    public MainView(IDialogService dialogs)
    {
        _dialogs = dialogs;
    }

    private async Task OnDeleteClicked()
    {
        var result = await _dialogs.ShowConfirmAsync(
            "Delete item",
            "Are you sure?",
            new DialogOptions { ButtonSet = DialogButtonSet.YesNo });

        if (result.Confirmed)
        {
            // delete logic
        }
    }
}
```

### Custom Override

Subclass `AvaloniaDialogService` and register it to customise any aspect of the dialog window:

```csharp
public class MyDialogService : AvaloniaDialogService
{
    // Override to change which window owns the dialog
    protected override Window? GetOwnerWindow() { ... }

    // Override to fully restyle the dialog window
    protected override Window BuildDialogWindow(string title, DialogOptions options) { ... }

    // Override to change button layout
    protected override Panel BuildButtonPanel(DialogOptions options,
        TaskCompletionSource<DialogResult> tcs, Window dialog) { ... }
}

// Register the custom implementation
services.AddAvaloniaDialogService<MyDialogService>();
```
