using System.Runtime.InteropServices;

namespace TaskbarAutoHideToggle;

/// <summary>
/// Registers a single system-wide hotkey (default Ctrl+Alt+T) via RegisterHotKey
/// and raises <see cref="Pressed"/> on a WM_HOTKEY message.
/// </summary>
internal sealed class GlobalHotkey : NativeWindow, IDisposable
{
    private const int WM_HOTKEY = 0x0312;
    private const int HotkeyId = 1;

    public const uint ModControl = 0x0002;
    public const uint ModAlt = 0x0001;
    public const uint VkT = 0x54;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(nint hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(nint hWnd, int id);

    private bool _registered;

    public event Action? Pressed;

    public GlobalHotkey()
    {
        CreateHandle(new CreateParams());
    }

    public bool Register(uint modifiers, uint key)
    {
        _registered = RegisterHotKey(Handle, HotkeyId, modifiers, key);
        return _registered;
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM_HOTKEY && m.WParam == HotkeyId)
        {
            Pressed?.Invoke();
        }
        base.WndProc(ref m);
    }

    public void Dispose()
    {
        if (_registered)
        {
            UnregisterHotKey(Handle, HotkeyId);
        }
        DestroyHandle();
    }
}
