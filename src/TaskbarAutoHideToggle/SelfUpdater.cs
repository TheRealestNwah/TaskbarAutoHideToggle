using System.Diagnostics;
using System.Net.Http;

namespace TaskbarAutoHideToggle;

/// <summary>
/// Downloads a new build and hands off to a detached PowerShell script that waits
/// for this process to exit, replaces the exe in place, and relaunches it.
/// </summary>
internal static class SelfUpdater
{
    public static async Task DownloadAndApplyAsync(UpdateInfo info, CancellationToken ct = default)
    {
        var currentExePath = Environment.ProcessPath
            ?? throw new InvalidOperationException("Could not determine the running exe's path.");

        var workDir = Path.Combine(Path.GetTempPath(), "TaskbarAutoHideToggle-update");
        Directory.CreateDirectory(workDir);
        var newExePath = Path.Combine(workDir, "TaskbarAutoHideToggle.new.exe");
        var scriptPath = Path.Combine(workDir, "apply-update.ps1");

        using (var client = new HttpClient())
        using (var response = await client.GetAsync(info.DownloadUrl, ct))
        {
            response.EnsureSuccessStatusCode();
            await using var fileStream = File.Create(newExePath);
            await response.Content.CopyToAsync(fileStream, ct);
        }

        var pid = Environment.ProcessId;
        var script = $$"""
            $ErrorActionPreference = 'Stop'
            try { Wait-Process -Id {{pid}} -Timeout 30 -ErrorAction SilentlyContinue } catch {}
            Start-Sleep -Milliseconds 500
            Copy-Item -Path '{{newExePath}}' -Destination '{{currentExePath}}' -Force
            Start-Process -FilePath '{{currentExePath}}'
            Remove-Item -Path '{{newExePath}}' -ErrorAction SilentlyContinue
            Remove-Item -Path $MyInvocation.MyCommand.Path -ErrorAction SilentlyContinue
            """;
        await File.WriteAllTextAsync(scriptPath, script, ct);

        Process.Start(new ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = $"-NoProfile -WindowStyle Hidden -ExecutionPolicy Bypass -File \"{scriptPath}\"",
            UseShellExecute = false,
            CreateNoWindow = true
        });
    }
}
