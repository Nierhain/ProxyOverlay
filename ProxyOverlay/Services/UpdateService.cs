using System.Diagnostics;
using Velopack;
using Velopack.Sources;

namespace ProxyOverlay.Services;

public sealed class UpdateService
{
    private const string GitHubRepository = "https://github.com/Nierhain/ProxyOverlay";

    public async Task CheckAndApplyAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // The current Velopack packaging below targets Windows. Linux and macOS
            // continue to use their existing portable packages.
            if (!OperatingSystem.IsWindows())
                return;

            var manager = new UpdateManager(new GithubSource(
                GitHubRepository,
                accessToken: "",
                prerelease: false,
                downloader: null,
                channel: "win",
                logger: null));
            if (!manager.IsInstalled)
                return;

            var update = await manager.CheckForUpdatesAsync(cancellationToken);
            if (update is null)
                return;

            await manager.DownloadUpdatesAsync(update, null, false, cancellationToken);
            manager.ApplyUpdatesAndRestart(Array.Empty<string>());
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            // Updating must never prevent the application from starting.
            Trace.WriteLine($"ProxyOverlay update check failed: {exception}");
        }
    }
}
