using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using ProxyOverlay.Services;

namespace ProxyOverlay.ViewModels;

public class DesignMainViewModel: MainViewModel 
{
    public DesignMainViewModel(): base(new DesignImageProcessor(), new DesignFilesService(), new DesignFileDialogService(), new DesignPreviewGenerator())
    {
        using var stream = AssetLoader.Open(
            new Uri("avares://ProxyOverlay/Assets/designer_preview.png"));

        PreviewImage = new Bitmap(stream);
    }
}

file class DesignPreviewGenerator: IPreviewGenerator
{
    public Bitmap CreatePreview(string imagePath, string overlayPath, uint maxWidth, uint maxHeight)
    {
        return new Bitmap("");
    }
}

file class DesignImageProcessor : IImageProcessor
{
    public Task ProcessAsync(string inputFolder, string outputFolder, IReadOnlyDictionary<string, string> overlayFiles, double overlayPercent,
        IProgress<ProcessProgress> progress, CancellationToken token = default)
    {
        return Task.CompletedTask;
    }
}

file class DesignFilesService : IFilesService
{
    public Task<string?> LoadFirstFile(string path)
    {
        return Task.FromResult<string?>(null);
    }
}

file class DesignFileDialogService : IFileDialogService
{
    public Task<string?> OpenOverlayAsync() => Task.FromResult<string?>(null);
    public Task<string?> OpenFolderAsync() => Task.FromResult<string?>(null);
}
