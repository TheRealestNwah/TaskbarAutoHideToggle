# Taskbar Auto-Hide Toggle

A Windows 11 tray app (C# / .NET 8, WinForms) that toggles taskbar auto-hide from a tray icon, with a global hotkey (Ctrl+Alt+T), a manual hide option and self-update. `SCOPE.md` holds the original design notes.

## Build, test, lint

```bash
dotnet restore
dotnet build -c Release
dotnet test -c Release
dotnet run --project src/TaskbarAutoHideToggle
```

CI (`windows-latest`, .NET 8.0.x, job `build`) runs restore, build and test on every PR. There is no separate lint step. The publish profile is `src/TaskbarAutoHideToggle/Properties/PublishProfiles/win-x64.pubxml` (self-contained single-file exe); `packaging/install.ps1` and `uninstall.ps1` do per-user installs.

## Layout

- `src/TaskbarAutoHideToggle/` — `Program.cs`, `TrayContext.cs` (tray icon and menu), `TaskbarInterop.cs` / `TaskbarVisibility.cs` (Win32 `SHAppBarMessage`), `GlobalHotkey.cs`, `StartupManager.cs`, `UpdateChecker.cs` / `SelfUpdater.cs`, `TrayIcons.cs`, `Resources/` (tray icons).
- `tests/TaskbarAutoHideToggle.Tests/` — unit tests for interop, startup and update logic.

## Gotchas

- Windows-only: it P/Invokes `user32.dll`, so CI and local builds need Windows.
- Auto-hide is changed through `SHAppBarMessage` (`ABM_SETSTATE`), not the registry plus an Explorer restart. Only the primary taskbar is handled.
- Releases publish a self-contained exe to GitHub Releases, and the app's updater reads from there; keep release asset names stable.
