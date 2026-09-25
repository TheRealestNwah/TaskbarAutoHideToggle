<#
.SYNOPSIS
    Removes the per-user install created by install.ps1.
.DESCRIPTION
    Stops the running app, removes it from the Start-with-Windows Run key,
    deletes the install directory and Start Menu shortcut.
#>
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$installDir = Join-Path $env:LOCALAPPDATA 'Programs\TaskbarAutoHideToggle'
$exeName = 'TaskbarAutoHideToggle.exe'
$runKey = 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Run'

Get-Process -Name 'TaskbarAutoHideToggle' -ErrorAction SilentlyContinue | Stop-Process -Force

if (Test-Path $runKey) {
    Remove-ItemProperty -Path $runKey -Name 'TaskbarAutoHideToggle' -ErrorAction SilentlyContinue
}

if (Test-Path $installDir) {
    Remove-Item -Recurse -Force $installDir
}

$startMenu = [Environment]::GetFolderPath('StartMenu')
$shortcutPath = Join-Path $startMenu 'Programs\Taskbar Auto-Hide Toggle.lnk'
if (Test-Path $shortcutPath) {
    Remove-Item -Force $shortcutPath
}

Write-Host "Uninstalled."
