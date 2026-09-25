namespace TaskbarAutoHideToggle;

/// <summary>
/// Generates the two tray icon states in code, avoiding checked-in .ico assets.
/// </summary>
internal static class TrayIcons
{
    public static Icon For(bool autoHideEnabled) =>
        autoHideEnabled ? BuildIcon(Color.MediumSeaGreen) : BuildIcon(Color.Gray);

    private static Icon BuildIcon(Color barColor)
    {
        using var bitmap = new Bitmap(16, 16);
        using (var g = Graphics.FromImage(bitmap))
        {
            g.Clear(Color.Transparent);
            using var brush = new SolidBrush(barColor);
            g.FillRectangle(brush, 1, 11, 14, 3);
        }

        nint hIcon = bitmap.GetHicon();
        try
        {
            return (Icon)Icon.FromHandle(hIcon).Clone();
        }
        finally
        {
            NativeMethods.DestroyIcon(hIcon);
        }
    }
}

file static class NativeMethods
{
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern bool DestroyIcon(nint hIcon);
}
