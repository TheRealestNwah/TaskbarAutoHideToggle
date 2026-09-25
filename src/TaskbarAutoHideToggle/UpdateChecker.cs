using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;

namespace TaskbarAutoHideToggle;

internal sealed record UpdateInfo(Version Version, string DownloadUrl);

/// <summary>
/// Checks GitHub Releases for a newer version than the running build.
/// </summary>
internal static class UpdateChecker
{
    private const string RepoApiUrl = "https://api.github.com/repos/TheRealestNwah/TaskbarAutoHideToggle/releases/latest";

    public static Version CurrentVersion =>
        Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 0, 0);

    /// <summary>Returns the latest release's info if it's newer than the running build, otherwise null.</summary>
    public static async Task<UpdateInfo?> CheckForUpdateAsync(CancellationToken ct = default)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("TaskbarAutoHideToggle", CurrentVersion.ToString()));
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));

        HttpResponseMessage response;
        try
        {
            response = await client.GetAsync(RepoApiUrl, ct);
        }
        catch (HttpRequestException)
        {
            return null; // offline or unreachable; treat as "no update available"
        }

        if (!response.IsSuccessStatusCode)
        {
            return null; // e.g. 404 when no release has been published yet
        }

        using var stream = await response.Content.ReadAsStreamAsync(ct);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
        var root = doc.RootElement;

        var tagName = root.GetProperty("tag_name").GetString() ?? "";
        if (!TryParseReleaseVersion(tagName, out var latestVersion) || !IsNewer(latestVersion, CurrentVersion))
        {
            return null;
        }

        var downloadUrl = root.GetProperty("assets")
            .EnumerateArray()
            .Select(a => a.GetProperty("browser_download_url").GetString())
            .FirstOrDefault(url => url is not null && url.EndsWith(".exe", StringComparison.OrdinalIgnoreCase));

        return downloadUrl is null ? null : new UpdateInfo(latestVersion, downloadUrl);
    }

    /// <summary>Parses a release tag like "v1.2.3" or "1.2.3" into a Version.</summary>
    internal static bool TryParseReleaseVersion(string tagName, out Version version) =>
        Version.TryParse(tagName.TrimStart('v', 'V'), out version!);

    internal static bool IsNewer(Version candidate, Version current) => candidate > current;
}
