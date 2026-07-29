using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImageMagick;
using ProxyOverlay.Models;

namespace ProxyOverlay.Services;

public sealed class ImageProcessor: IImageProcessor
{
    public async Task ProcessAsync(
        string inputFolder,
        string outputFolder,
        IReadOnlyDictionary<string, string> overlayFiles,
        double overlayPercent,
        IProgress<ProcessProgress> progress,
        CancellationToken token = default)
    {
        Directory.CreateDirectory(outputFolder);

        var files = Directory.EnumerateFiles(inputFolder)
            .Where(IsImage)
            .ToList();

        var overlayCache = new Dictionary<string, MagickImage>(StringComparer.OrdinalIgnoreCase);

        try
        {
            for (var i = 0; i < files.Count; i++)
            {
                token.ThrowIfCancellationRequested();

                var overlayFile = overlayFiles[files[i]];
                if (!overlayCache.TryGetValue(overlayFile, out var overlayOriginal))
                {
                    overlayOriginal = ImageSourceLoader.Load(overlayFile);
                    overlayCache.Add(overlayFile, overlayOriginal);
                }

                ProcessImage(files[i], outputFolder, overlayOriginal, overlayPercent);

                progress.Report(new ProcessProgress(i + 1, files.Count));
            }
        }
        finally
        {
            foreach (var overlay in overlayCache.Values)
            {
                overlay.Dispose();
            }
        }

        await Task.CompletedTask;
    }
    
    private static void ProcessImage(
        string file,
        string outputFolder,
        MagickImage overlayOriginal,
        double percent)
    {
        using var image = new MagickImage(file);

        image.AutoOrient();

        using var overlay = overlayOriginal.Clone();

        var targetWidth =
            (uint)(image.Width * (percent / 100));

        overlay.Resize(targetWidth, 0);

        image.Composite(
            overlay,
            Gravity.South,
            CompositeOperator.Over);

        var output =
            Path.Combine(outputFolder,
                Path.GetFileName(file));

        image.Write(output);
    }
    
    private static readonly HashSet<string> Extensions =
    [
        ".jpg",
        ".jpeg",
        ".png",
        ".bmp",
        ".gif",
        ".webp",
        ".tif",
        ".tiff"
    ];

    private static bool IsImage(string file)
    {
        return Extensions.Contains(
            Path.GetExtension(file).ToLowerInvariant());
    }
}
