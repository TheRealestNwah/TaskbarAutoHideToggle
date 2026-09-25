namespace TaskbarAutoHideToggle.Tests;

public class UpdateCheckerTests
{
    [Theory]
    [InlineData("v1.2.3", true, "1.2.3")]
    [InlineData("1.2.3", true, "1.2.3")]
    [InlineData("V0.1.0", true, "0.1.0")]
    [InlineData("not-a-version", false, null)]
    [InlineData("", false, null)]
    public void TryParseReleaseVersion_HandlesTagFormats(string tag, bool expectedOk, string? expectedVersion)
    {
        var ok = UpdateChecker.TryParseReleaseVersion(tag, out var version);

        Assert.Equal(expectedOk, ok);
        if (expectedOk)
        {
            Assert.Equal(new Version(expectedVersion!), version);
        }
    }

    [Theory]
    [InlineData("1.1.0", "1.0.0", true)]
    [InlineData("1.0.0", "1.0.0", false)]
    [InlineData("0.9.0", "1.0.0", false)]
    public void IsNewer_ComparesVersionsCorrectly(string candidate, string current, bool expected)
    {
        Assert.Equal(expected, UpdateChecker.IsNewer(new Version(candidate), new Version(current)));
    }
}
