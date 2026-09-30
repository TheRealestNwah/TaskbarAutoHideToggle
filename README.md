# Taskbar Auto-Hide Toggle

A lightweight Windows 11 tray app that lets you toggle taskbar auto-hide without digging through Settings.

> **Built with AI.** Taskbar Auto-Hide Toggle's code, tests and documentation were written by
> Claude, an AI model from Anthropic, directed and tested by the maintainer.
> See [AI disclosure](#ai-disclosure).

## Usage

- Left-click the tray icon, use the right-click menu, or press **Ctrl+Alt+T**, to toggle auto-hide.
- "Hide taskbar" in the menu hides the taskbar outright (independent of auto-hide).
- The icon's bar is green when auto-hide is on, gray when off.
- "Start with Windows" in the menu launches the app on login.

## Install

**Quickest way:** grab the latest `TaskbarAutoHideToggle.exe` from [Releases](https://github.com/TheRealestNwah/TaskbarAutoHideToggle/releases/latest) and run it. No install, no .NET runtime needed — it's self-contained. Windows SmartScreen may warn since the exe isn't code-signed; choose "More info" > "Run anyway" if you trust the source.

**From source**, if you'd rather build it yourself: requires the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (only for building — the installed app is self-contained and needs no separate runtime).

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

## AI disclosure

Taskbar Auto-Hide Toggle was built with [Claude Code](https://claude.com/claude-code), Anthropic's
AI coding assistant. Claude wrote the code, tests and documentation. The
maintainer ([@TheRealestNwah](https://github.com/TheRealestNwah)) decided what
it should do, tested it, and made the release decisions. Commits written with
Claude carry a `Co-Authored-By: Claude` trailer, so the git history shows which
changes were AI-written.

## Support

Everything on my GitHub is free of charge and open source. If you find it
useful and want to leave a tip or buy me a coffee, you can do that at
[ko-fi.com/morrowheat23](https://ko-fi.com/morrowheat23). It's appreciated,
never expected.

## License

MIT — see [LICENSE](LICENSE).
