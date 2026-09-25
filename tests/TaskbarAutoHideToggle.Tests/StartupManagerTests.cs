namespace TaskbarAutoHideToggle.Tests;

public class StartupManagerTests
{
    [Theory]
    [InlineData(@"C:\Program Files\TaskbarAutoHideToggle\TaskbarAutoHideToggle.exe", "\"C:\\Program Files\\TaskbarAutoHideToggle\\TaskbarAutoHideToggle.exe\"")]
    [InlineData(@"C:\bin\app.exe", "\"C:\\bin\\app.exe\"")]
    public void QuotedRunValue_WrapsPathInQuotes(string exePath, string expected)
    {
        Assert.Equal(expected, StartupManager.QuotedRunValue(exePath));
    }
}
