using System.Runtime.InteropServices;

namespace TaskbarAutoHideToggle;

/// <summary>
/// P/Invoke wrapper around the Shell AppBar API for reading and setting
/// the primary taskbar's auto-hide state.
/// </summary>
internal static class TaskbarInterop
{
    private const int ABM_GETSTATE = 0x4;
    private const int ABM_SETSTATE = 0xA;
    internal const int ABS_AUTOHIDE = 0x1;

    [StructLayout(LayoutKind.Sequential)]
    private struct APPBARDATA
    {
        public int cbSize;
        public nint hWnd;
        public uint uCallbackMessage;
        public uint uEdge;
        public RECT rc;
        public nint lParam;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left, Top, Right, Bottom;
    }

    [DllImport("shell32.dll")]
    private static extern nint SHAppBarMessage(uint dwMessage, ref APPBARDATA pData);

    public static bool IsAutoHideEnabled()
    {
        var data = new APPBARDATA { cbSize = Marshal.SizeOf<APPBARDATA>() };
        var state = (int)SHAppBarMessage(ABM_GETSTATE, ref data);
        return HasAutoHideFlag(state);
    }

    public static void SetAutoHide(bool enabled)
    {
        var data = new APPBARDATA
        {
            cbSize = Marshal.SizeOf<APPBARDATA>(),
            lParam = AutoHideLParam(enabled)
        };
        SHAppBarMessage(ABM_SETSTATE, ref data);
    }

    /// <summary>Whether the ABS_AUTOHIDE bit is set in a state value from ABM_GETSTATE.</summary>
    internal static bool HasAutoHideFlag(int state) => (state & ABS_AUTOHIDE) != 0;

    /// <summary>The lParam to pass to ABM_SETSTATE for the desired auto-hide state.</summary>
    internal static int AutoHideLParam(bool enabled) => enabled ? ABS_AUTOHIDE : 0;
}
