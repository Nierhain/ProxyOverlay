using ProxyOverlay.Models;
using ProxyOverlay.Services;

namespace ProxyOverlay.ViewModels;

internal sealed class DesignImageProcessor : IImageProcessor
{
    public Task ProcessAsync(
        string inputFolder,
        string outputFolder,
        IReadOnlyDictionary<string, string> overlayFiles,
        double overlayPercent,
        IProgress<ProcessProgress> progress,
        CancellationToken token = default)
    {
        return Task.CompletedTask;
    }
}
