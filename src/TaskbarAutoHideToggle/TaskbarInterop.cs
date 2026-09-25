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
    private const int ABS_AUTOHIDE = 0x1;

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
        return (state & ABS_AUTOHIDE) != 0;
    }

    public static void SetAutoHide(bool enabled)
    {
        var data = new APPBARDATA
        {
            cbSize = Marshal.SizeOf<APPBARDATA>(),
            lParam = enabled ? ABS_AUTOHIDE : 0
        };
        SHAppBarMessage(ABM_SETSTATE, ref data);
    }
}
