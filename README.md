# MVVMUniversal - README

## Overview

MVVMUniversal provides a small, portable MVVM foundation (`Core.MVVM`) and a platform-agnostic library for app concerns and API clients (`Core.Lib`). The goal is to write ViewModels and business logic once and reuse them across multiple UI platforms (MAUI, Blazor, WinForms, Xamarin, etc.). This README explains the responsibilities of each core component, available classes, and how to wire platform-specific implementations (Settings, Navigation) using the lightweight DependencyService.

## Core.MVVM (portable, reusable)

### Purpose

Reusable base classes and patterns for implementing MVVM across platforms.

### Key types and responsibilities

- `ObservableObject`
  - Implements `INotifyPropertyChanged` and `INotifyDataErrorInfo`.
  - Provides `Set(ref field, newValue, [CallerMemberName])` to update fields, validate and raise change notifications.
  - Tracks validation errors and exposes `MessagesErrors` / `ValidationSumary`.

- `RelayCommand` and `RelayCommand<T>`
  - `ICommand` implementations with sync and async overloads.
  - Optional auto-disable while command is processing.
  - Dependency wiring to auto-refresh `CanExecute` when related properties change.

- `ViewModelBase`
  - Extends `ObservableObject` with common flags (`Loading`, `Processing`).

- Patterns (`namespace Codeland.Core.MVVM.Pattern`)
  - `ISelectedItem<T>, ICrud<T>`: interfaces for common list/CRUD patterns.
  - `ViewModelWithBL<T>`: small helper that holds a business layer (BL) instance.
  - `ViewModelCRUD<T>`: a reusable CRUD viewmodel implementation using `RelayCommand` and `ICrud<T>`.

### Guideline

Keep `Core.MVVM` fully platform-independent so ViewModels are testable and reusable.

## Core.Lib (portable app behaviors and API clients)

### Purpose

Provide shared app services, models and API client helpers that are UI-framework agnostic.

### Key types and responsibilities

- `Core.Lib.OS` (platform abstractions)
  - `INavigationService`: abstract navigation operations (`NavigateTo`, `GoBack`, `Home`, `PushModal`, `PopModal`, `NavigateToUrl`).
  - `ISettingsStorage`: abstract persistent settings read/write per property (`GetValue<T>`, `SetValue<T>`).
  - `PagesKeys`: string constants for navigation targets (`Login`, `Crud`, `Other`).

- `Settings` (`Core.Helpers.Settings`)
  - A portable wrapper that exposes user-facing settings as properties and stores them through `ISettingsStorage`.
  - Example property: `WebAPIUrl` reads/writes via the registered `ISettingsStorage` implementation.

- Web API clients and business layer
  - `WebApiClient`: base HTTP helper initialized using `Settings.Current.WebAPIUrl` and a controller path.
  - `SecurityApi`: example API wrapper invoking security endpoints.
  - `SecurityBL`: business-level class that uses `SecurityApi` to implement login/validation; used by ViewModels.

## DependencyService (`Codeland.Core.OS.DependencyService`)

A small DI/resolution helper used by the samples. It supports simple lifetimes (Singleton, Scoped, Transient).

- API examples:
  - Register: `Codeland.Core.OS.DependencyService.Register<PlatformNavigationService, INavigationService>(Codeland.Core.OS.DependencyService.ServiceLifetime.Singleton);`
  - Resolve: `var nav = Codeland.Core.OS.DependencyService.Get<INavigationService>();`

## Platform-specific implementations

Rule of thumb: implement only thin adapters over platform APIs and register them at app startup.

Examples in this repository:

- **MAUI (MAUIApp)**
  - `MAUIApp.OS.NavigationService` : implements `Core.Lib.OS.INavigationService` using `Shell.Navigation` for push/pop.
  - `MAUIApp.OS.SettingsStorage` : implements `Core.Lib.OS.ISettingsStorage` using `Microsoft.Maui.Storage.Preferences`.
  - Registration: in `MauiProgram.CreateMauiApp()` or App constructor register implementations with `Codeland.Core.OS.DependencyService`. `AppShell` constructor sets the `Shell.Navigation` instance into the registered `NavigationService` when needed.

- **Blazor (BlazorApp)**
  - `BlazorApp.OS.NavigationService` : implements `INavigationService` using `NavigationManager`; after the Blazor host is ready set `NavigationManager` into the registered implementation.
  - `SettingsStorage` for Blazor can read settings.json or use browser storage.

- **WinForms (WinFormsApp)**
  - `WinFormsApp.OS.NavigationService` : implement navigation using forms, panels, or windows and register it at `Program.Main` before creating the main form.
  - `SettingsStorage` uses Registry, config file or your preferred storage.

## How to register platform services (examples)

- **MAUI (`MauiProgram.CreateMauiApp` / App)**
  ```csharp
  CoreOS.DependencyService.Register<MAUIApp.OS.NavigationService, INavigationService>(CoreOS.DependencyService.ServiceLifetime.Singleton);
  CoreOS.DependencyService.Register<MAUIApp.OS.SettingsStorage, ISettingsStorage>();
  ```
  - In `AppShell` constructor set navigation instance when your `NavigationService` needs `Shell.Navigation`:
    ```csharp
    (CoreOS.DependencyService.Get<INavigationService>() as MAUIApp.OS.NavigationService).Navigation = this.Navigation;
    ```

- **Blazor (`Program.Main`)**
  ```csharp
  CoreOS.DependencyService.Register<BlazorApp.OS.NavigationService, INavigationService>(...);
  ```
  - After the Blazor host is built, set `NavigationManager` into the registered service.

- **WinForms (`Program.Main`)**
  ```csharp
  CoreOS.DependencyService.Register<WinFormsApp.OS.NavigationService, INavigationService>(...);
  ```
  - Resolve and wire forms/panels as needed.

## Using the Core libraries from ViewModels

- Resolve platform services when you need them in a ViewModel or platform-agnostic code:
  ```csharp
  await Codeland.Core.OS.DependencyService.Get<INavigationService>().NavigateTo(PagesKeys.Crud);
  ```

- Use `Settings.Current` to read/write settings across platforms:
  ```csharp
  Settings.Current.WebAPIUrl = "https://api.example.com/";
  ```

## Login flow (example)

1. `LoginViewModel.LoginCommand` calls `SecurityBL.Login(user, password)`.
2. `SecurityBL` uses `SecurityApi` to call the server (`WebApiClient` uses `Settings.Current.WebAPIUrl`).
3. If login succeeds, the ViewModel requests navigation with `INavigationService` to the target page.

## Best practices and notes

- Keep `Core.*` code UI-framework free. Only platform projects reference MAUI/Blazor/WinForms.
- Keep platform implementations thin and focus on mapping platform APIs to the small OS interfaces.
- The provided `DependencyService` is intentionally small. If you prefer `Microsoft.Extensions.DependencyInjection`, you can replace it by registering platform services into that container and adapt the `Resolve` calls in Core.

## Extending the sample

- To add another platform, implement `INavigationService` and `ISettingsStorage` for that platform and register them at startup.
- Optionally add more app-level adapters (clipboard, file picker, telemetry) using the same pattern.

## Support

If you want, I can add minimal example code showing the login flow wired end-to-end for MAUI (View + ViewModel + Navigation registration) or create unit-test examples for Core.MVVM types.