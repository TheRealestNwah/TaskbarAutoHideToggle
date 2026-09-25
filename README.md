# Taskbar Auto-Hide Toggle

A lightweight Windows 11 tray app that lets you toggle taskbar auto-hide without digging through Settings.

## Usage

- Left-click the tray icon, use the right-click menu, or press **Ctrl+Alt+T**, to toggle auto-hide.
- "Hide taskbar" in the menu hides the taskbar outright (independent of auto-hide).
- The icon's bar is green when auto-hide is on, gray when off.
- "Start with Windows" in the menu launches the app on login.

## Install

Requires the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (only for building — the installed app is self-contained and needs no separate runtime).

```powershell
.\packaging\install.ps1
```

This publishes a self-contained single-file exe, copies it to `%LocalAppData%\Programs\TaskbarAutoHideToggle`, and adds a Start Menu shortcut. No admin rights required. To remove it: `.\packaging\uninstall.ps1`.

## Build & run from source

```
dotnet build -c Release
dotnet run --project src/TaskbarAutoHideToggle
```

## How it works

Toggling calls the Shell AppBar API (`SHAppBarMessage` with `ABM_SETSTATE`/`ABM_GETSTATE`) directly — the same mechanism Explorer itself uses — so the change applies instantly with no Explorer restart. See [`TaskbarInterop.cs`](src/TaskbarAutoHideToggle/TaskbarInterop.cs).

See [SCOPE.md](SCOPE.md) for the full design scope and roadmap.
