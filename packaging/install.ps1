<#
.SYNOPSIS
    Builds a self-contained single-file exe and installs it for the current user.
.DESCRIPTION
    Publishes TaskbarAutoHideToggle, copies it to %LocalAppData%\Programs\TaskbarAutoHideToggle,
    and creates a Start Menu shortcut. No admin rights required (per-user install only).
#>
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
$project = Join-Path $repoRoot 'src\TaskbarAutoHideToggle\TaskbarAutoHideToggle.csproj'
$installDir = Join-Path $env:LOCALAPPDATA 'Programs\TaskbarAutoHideToggle'
$exeName = 'TaskbarAutoHideToggle.exe'

# PATH sometimes resolves an x86 `dotnet` stub with no SDK installed ahead of
# the real 64-bit one; prefer an install that actually reports an SDK.
$dotnet = Get-Command dotnet -All -ErrorAction SilentlyContinue |
    Where-Object { & $_.Source --list-sdks 2>$null } |
    Select-Object -First 1 -ExpandProperty Source
if (-not $dotnet) { throw "Could not find a dotnet install with an SDK on PATH." }

Write-Host "Publishing self-contained build (using $dotnet)..."
& $dotnet publish $project -p:PublishProfile=win-x64 -c Release
if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed" }

$publishDir = Join-Path $repoRoot 'src\TaskbarAutoHideToggle\bin\Release\net8.0-windows\win-x64\publish'

New-Item -ItemType Directory -Force -Path $installDir | Out-Null
Copy-Item -Path (Join-Path $publishDir $exeName) -Destination $installDir -Force

$shell = New-Object -ComObject WScript.Shell
$startMenu = [Environment]::GetFolderPath('StartMenu')
$shortcut = $shell.CreateShortcut((Join-Path $startMenu 'Programs\Taskbar Auto-Hide Toggle.lnk'))
$shortcut.TargetPath = Join-Path $installDir $exeName
$shortcut.WorkingDirectory = $installDir
$shortcut.Description = 'Toggle Windows taskbar auto-hide from the tray'
$shortcut.Save()

Write-Host "Installed to $installDir"
Write-Host "Shortcut added to Start Menu. Launch it, then optionally enable 'Start with Windows' from its tray menu."
