using Microsoft.Win32;

namespace TaskbarAutoHideToggle;

/// <summary>
/// Toggles launch-on-login via the HKCU Run registry key.
/// </summary>
internal static class StartupManager
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string ValueName = "TaskbarAutoHideToggle";

    public static bool IsEnabled()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: false);
        return key?.GetValue(ValueName) is not null;
    }

    public static void SetEnabled(bool enabled)
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true)
            ?? Registry.CurrentUser.CreateSubKey(RunKeyPath);

        if (enabled)
        {
            key.SetValue(ValueName, QuotedRunValue(Environment.ProcessPath ?? string.Empty));
        }
        else
        {
            key.DeleteValue(ValueName, throwOnMissingValue: false);
        }
    }

    /// <summary>Wraps an exe path in quotes so the Run key value is safe even with spaces in the path.</summary>
    internal static string QuotedRunValue(string exePath) => $"\"{exePath}\"";
}
