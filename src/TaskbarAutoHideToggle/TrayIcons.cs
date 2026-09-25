using System.Reflection;

namespace TaskbarAutoHideToggle;

/// <summary>
/// Loads the embedded on/off tray icon states.
/// </summary>
internal static class TrayIcons
{
    private static readonly Icon OnIcon = Load("tray-on.ico");
    private static readonly Icon OffIcon = Load("tray-off.ico");

    public static Icon For(bool autoHideEnabled) => autoHideEnabled ? OnIcon : OffIcon;

    private static Icon Load(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"{assembly.GetName().Name}.Resources.{fileName}";
        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded resource '{resourceName}' not found.");
        return new Icon(stream);
    }
}
