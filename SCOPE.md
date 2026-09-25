# Taskbar Auto-Hide Toggle — Scope

## Goal
A small always-running tray app for Windows 11 that lets the user toggle taskbar auto-hide on/off from a tray icon, without opening Settings.

## Stack
C# / .NET 8, WinForms (NotifyIcon + ContextMenuStrip), published as a self-contained single-file exe. No installer needed initially — just an exe (optionally a shortcut in the Startup folder for launch-on-login).

WinForms over WPF: this app is tray-icon-only with no real window, so WinForms' lighter NotifyIcon/menu plumbing is a better fit and keeps the exe smaller.

## How toggling actually works
Windows 11's taskbar auto-hide state is controlled via the `SHAppBarMessage` Win32 API (`user32.dll`):
- Read current state: `SHAppBarMessage(ABM_GETSTATE, ...)` → returns flags, check `ABS_AUTOHIDE` bit.
- Set new state: `SHAppBarMessage(ABM_SETSTATE, ...)` with `ABS_AUTOHIDE` bit set/cleared in `lParam`.

This takes effect immediately, no Explorer restart needed. (The alternative — writing `HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\StuckRects3` and restarting explorer.exe — is heavier-handed and flashes the desktop, so the AppBar API is preferred.)

On multi-monitor setups, Windows 11 tracks auto-hide per-monitor via `ABM_GETSTATE`/`ABM_SETSTATE` with an `APPBARDATA.hWnd` target — scope v1 to the primary taskbar only, note multi-monitor as a stretch item.

## MVP features
- Tray icon, always running in background (no visible window/taskbar entry).
- Left-click or menu item: toggle auto-hide on/off.
- Icon reflects current state (two icon variants: hidden/visible) or a checkmark in the context menu.
- Right-click context menu: "Auto-hide: On/Off" (toggle), "Start with Windows" (checkbox), "Exit".
- Persist "start with Windows" via a Startup-folder shortcut (or `HKCU\...\Run` registry key).

## Out of scope for v1
- Installer/MSIX packaging.
- Per-monitor auto-hide control.
- Global hotkey to toggle (possible stretch goal).
- Settings UI beyond the tray context menu.

## Project layout (planned)
```
TaskbarAutoHideToggle/
  TaskbarAutoHideToggle.sln
  src/
    TaskbarAutoHideToggle/
      Program.cs           # entry point, tray icon setup
      TaskbarInterop.cs    # SHAppBarMessage P/Invoke wrapper
      TrayContext.cs       # ApplicationContext driving the NotifyIcon + menu
      StartupManager.cs    # start-with-Windows toggle
      Resources/           # tray icons (on/off states)
  README.md
```

## Open questions before implementation
- App name/branding for the tray tooltip and Startup shortcut?
- Any preference on icon style (simple monochrome vs colored)?
- Ship as single portable exe only, or also want a Setup/installer down the line?

## Next step
On the go-ahead, scaffold the .NET WinForms project per the layout above and implement `TaskbarInterop` + tray icon first, since that's the core mechanism to validate.
