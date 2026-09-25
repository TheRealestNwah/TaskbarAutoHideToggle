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
    private readonly ToolStripMenuItem _updateItem;
    private readonly GlobalHotkey _hotkey;
    private UpdateInfo? _pendingUpdate;

    public TrayContext()
    {
        _autoHideItem = new ToolStripMenuItem("Auto-hide taskbar (Ctrl+Alt+T)", null, OnToggleAutoHide);
        _hideTaskbarItem = new ToolStripMenuItem("Hide taskbar", null, OnToggleHideTaskbar);
        _startupItem = new ToolStripMenuItem("Start with Windows", null, OnToggleStartup)
        {
            Checked = StartupManager.IsEnabled()
        };
        _updateItem = new ToolStripMenuItem("Check for updates...", null, OnCheckForUpdates);
        var exitItem = new ToolStripMenuItem("Exit", null, OnExit);

        var menu = new ContextMenuStrip();
        menu.Items.Add(_autoHideItem);
        menu.Items.Add(_hideTaskbarItem);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(_startupItem);
        menu.Items.Add(_updateItem);
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
        _ = CheckForUpdateAsync(silent: true);
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

    private void OnCheckForUpdates(object? sender, EventArgs e) => _ = CheckForUpdateAsync(silent: false);

    private async Task CheckForUpdateAsync(bool silent)
    {
        if (_pendingUpdate is not null)
        {
            PromptToInstallUpdate(_pendingUpdate);
            return;
        }

        var update = await UpdateChecker.CheckForUpdateAsync();
        if (update is null)
        {
            if (!silent)
            {
                MessageBox.Show("You're on the latest version.", "Taskbar Auto-Hide Toggle",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return;
        }

        _pendingUpdate = update;
        _updateItem.Text = $"Update to v{update.Version}...";
        PromptToInstallUpdate(update);
    }

    private void PromptToInstallUpdate(UpdateInfo update)
    {
        var result = MessageBox.Show(
            $"Version {update.Version} is available (you're on {UpdateChecker.CurrentVersion}). Download and install now? The app will restart.",
            "Update available", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

        if (result == DialogResult.Yes)
        {
            _ = InstallUpdateAsync(update);
        }
    }

    private async Task InstallUpdateAsync(UpdateInfo update)
    {
        try
        {
            await SelfUpdater.DownloadAndApplyAsync(update);
            OnExit(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Update failed: {ex.Message}", "Taskbar Auto-Hide Toggle",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
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
