using ProxyOverlay.Models;

namespace ProxyOverlay.Services;

public interface IImageProcessor
{
    Task ProcessAsync(
        string inputFolder,
        string outputFolder,
        IReadOnlyDictionary<string, string> overlayFiles,
        double overlayPercent,
        IProgress<ProcessProgress> progress,
        CancellationToken token = default);
}
