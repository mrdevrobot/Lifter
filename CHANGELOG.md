# Change Log

All notable changes to this project will be documented in this file. See [versionize](https://github.com/versionize/versionize) for commit guidelines.

<a name="2.0.1"></a>
## [2.0.1](https://www.github.com/lucafabbri/Lifter/releases/tag/v2.0.1) (2026-05-09)

### Changes

* Added dedicated NuGet README for each package (`Lifter.Core`, `Lifter.Avalonia`, `Lifter.Blazor`, `Lifter.Maui`)
* Updated package descriptions and tags in all `.csproj` files

---

<a name="2.0.0"></a>
## [2.0.0](https://www.github.com/lucafabbri/Lifter/releases/tag/v2.0.0) (2026-05-09)

### Breaking Changes

* **Target framework upgraded to .NET 10** — all packages now require `net10.0` (or `net10.0-*` for MAUI)
* **Avalonia upgraded to 12.0** — `Lifter.Avalonia` now targets Avalonia 12.0.2; `SystemDecorations` replaced by `WindowDecorations`, `TextBox.Watermark` replaced by `TextBox.PlaceholderText`
* `Avalonia.Diagnostics` replaced by `ProDiagnostics 12.0.0` (no Avalonia.Diagnostics v12 release)

### Features

* **`IDialogService`** — New cross-platform dialog abstraction in `Lifter.Core.Dialog`:
  * `ShowMessageAsync` — informational message dialog
  * `ShowConfirmAsync` — yes/no or ok/cancel confirmation dialog
  * `ShowAsync` — generic dialog with custom content
  * `DialogResult`, `DialogButton`, `DialogButtonSet`, `DialogOptions` supporting types
  * `NullDialogService` no-op implementation for testing/defaults
* **`AvaloniaDialogService`** — Avalonia implementation with fully virtual override points (`GetOwnerWindow`, `BuildDialogWindow`, `BuildButtonPanel`, `ShowDialogWindowAsync`)
* **`BlazorDialogService`** — Blazor WASM implementation via JS interop (`alert` / `confirm`)
* **`MauiDialogService`** — MAUI implementation via `Page.DisplayAlert`
* **DI extensions** — `AddAvaloniaDialogService()`, `AddBlazorDialogService()`, `AddMauiDialogService()` with generic overloads for custom subclasses
* **`Lifter.Examples.Avalonia`** — New sample project demonstrating background services and `IDialogService` with four dialog scenarios
* **Blazor sample updated** — `DialogDemo` page added to `Lifter.Examples.Blazor` at `/dialogs`

---

<a name="1.1.0"></a>
## [1.1.0](https://www.github.com/lucafabbri/Lifter/releases/tag/v1.1.0) (2026-01-15)

### Features

* **Lifter.Avalonia** - New package for Avalonia UI applications with HostedApplication base class
* **Documentation Site** - GitHub Pages documentation with versioning at https://lucafabbri.github.io/Lifter/
* **GitHub Actions** - Updated publish workflow with automated docs deployment

### Changes

* Updated all packages to version 1.1.0
* Enhanced README with Lifter.Avalonia examples and NuGet badges

---


<a name="1.0.2"></a>
## [1.0.2](https://www.github.com/lucafabbri/Lifter/releases/tag/v1.0.2) (2025-09-09)

<a name="1.0.1"></a>
## [1.0.1](https://www.github.com/lucafabbri/Lifter/releases/tag/v1.0.1) (2025-09-09)

<a name="1.0.0"></a>
## [1.0.0](https://www.github.com/lucafabbri/Lifter/releases/tag/v1.0.0) (2025-09-05)

### Features

* Lifter Maui Hosted service management ([143c555](https://www.github.com/lucafabbri/Lifter/commit/143c555895bd4c7c19cdd3b7ab199e6a77c99d0a))

