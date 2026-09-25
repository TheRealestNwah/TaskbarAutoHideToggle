namespace TaskbarAutoHideToggle.Tests;

public class TaskbarInteropTests
{
    [Theory]
    [InlineData(0, false)]
    [InlineData(TaskbarInterop.ABS_AUTOHIDE, true)]
    [InlineData(0x2, false)] // some other unrelated appbar state flag
    [InlineData(TaskbarInterop.ABS_AUTOHIDE | 0x2, true)] // auto-hide plus another flag
    public void HasAutoHideFlag_ReadsTheAutoHideBit(int state, bool expected)
    {
        Assert.Equal(expected, TaskbarInterop.HasAutoHideFlag(state));
    }

    [Theory]
    [InlineData(true, TaskbarInterop.ABS_AUTOHIDE)]
    [InlineData(false, 0)]
    public void AutoHideLParam_EncodesTheDesiredState(bool enabled, int expected)
    {
        Assert.Equal(expected, TaskbarInterop.AutoHideLParam(enabled));
    }
}
