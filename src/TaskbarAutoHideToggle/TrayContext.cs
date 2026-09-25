namespace TaskbarAutoHideToggle;

/// <summary>
/// Drives the tray icon and its context menu; owns no visible window.
/// </summary>
internal sealed class TrayContext : ApplicationContext
{
    private readonly NotifyIcon _notifyIcon;
    private readonly ToolStripMenuItem _autoHideItem;
    private readonly ToolStripMenuItem _hideTaskbarItem;
    private readonly ToolStripMenuItem _startupItem;
    private readonly GlobalHotkey _hotkey;

    public TrayContext()
    {
        _autoHideItem = new ToolStripMenuItem("Auto-hide taskbar (Ctrl+Alt+T)", null, OnToggleAutoHide);
        _hideTaskbarItem = new ToolStripMenuItem("Hide taskbar", null, OnToggleHideTaskbar);
        _startupItem = new ToolStripMenuItem("Start with Windows", null, OnToggleStartup)
        {
            Checked = StartupManager.IsEnabled()
        };
        var exitItem = new ToolStripMenuItem("Exit", null, OnExit);

        var menu = new ContextMenuStrip();
        menu.Items.Add(_autoHideItem);
        menu.Items.Add(_hideTaskbarItem);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(_startupItem);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(exitItem);

        _notifyIcon = new NotifyIcon
        {
            ContextMenuStrip = menu,
            Visible = true
        };
        _notifyIcon.MouseClick += OnTrayIconClick;

        _hotkey = new GlobalHotkey();
        _hotkey.Pressed += ToggleAutoHide;
        _hotkey.Register(GlobalHotkey.ModControl | GlobalHotkey.ModAlt, GlobalHotkey.VkT);

        RefreshState();
    }

    private void OnTrayIconClick(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            ToggleAutoHide();
        }
    }

    private void OnToggleAutoHide(object? sender, EventArgs e) => ToggleAutoHide();

    private void ToggleAutoHide()
    {
        TaskbarInterop.SetAutoHide(!TaskbarInterop.IsAutoHideEnabled());
        RefreshState();
    }

    private void OnToggleHideTaskbar(object? sender, EventArgs e)
    {
        TaskbarVisibility.SetVisible(!TaskbarVisibility.IsVisible());
        RefreshState();
    }

    private void OnToggleStartup(object? sender, EventArgs e)
    {
        var enabled = !StartupManager.IsEnabled();
        StartupManager.SetEnabled(enabled);
        _startupItem.Checked = enabled;
    }

    private void OnExit(object? sender, EventArgs e)
    {
        // Don't strand the user without a taskbar if the app exits while it's manually hidden.
        TaskbarVisibility.SetVisible(true);
        _notifyIcon.Visible = false;
        _hotkey.Dispose();
        Application.Exit();
    }

    private void RefreshState()
    {
        var autoHide = TaskbarInterop.IsAutoHideEnabled();
        var hidden = !TaskbarVisibility.IsVisible();
        _autoHideItem.Checked = autoHide;
        _hideTaskbarItem.Checked = hidden;
        _notifyIcon.Icon = TrayIcons.For(autoHide);
        _notifyIcon.Text = $"Taskbar auto-hide: {(autoHide ? "On" : "Off")}{(hidden ? " (hidden)" : "")}";
    }
}
