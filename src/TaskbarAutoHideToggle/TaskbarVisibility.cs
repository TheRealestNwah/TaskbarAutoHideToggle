using System.Runtime.InteropServices;

namespace TaskbarAutoHideToggle;

/// <summary>
/// Shows/hides the taskbar window(s) outright, independent of auto-hide.
/// Targets the primary taskbar (Shell_TrayWnd) and any secondary-monitor
/// taskbars (Shell_SecondaryTrayWnd).
/// </summary>
internal static class TaskbarVisibility
{
    private const int SW_HIDE = 0;
    private const int SW_SHOW = 5;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern nint FindWindow(string lpClassName, string? lpWindowName);

    [DllImport("user32.dll")]
    private static extern nint FindWindowEx(nint hwndParent, nint hwndChildAfter, string lpszClass, string? lpszWindow);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(nint hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(nint hWnd);

    public static bool IsVisible()
    {
        var primary = FindWindow("Shell_TrayWnd", null);
        return primary == 0 || IsWindowVisible(primary);
    }

    public static void SetVisible(bool visible)
    {
        var cmd = visible ? SW_SHOW : SW_HIDE;
        foreach (var hWnd in EnumerateTaskbarWindows())
        {
            ShowWindow(hWnd, cmd);
        }
    }

    private static IEnumerable<nint> EnumerateTaskbarWindows()
    {
        var primary = FindWindow("Shell_TrayWnd", null);
        if (primary != 0)
        {
            yield return primary;
        }

        nint secondary = 0;
        while ((secondary = FindWindowEx(0, secondary, "Shell_SecondaryTrayWnd", null)) != 0)
        {
            yield return secondary;
        }
    }
}
