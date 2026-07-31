using Avalonia.Platform.Storage;

namespace ProxyOverlay.Services;

public sealed class AvaloniaFileDialogService(IStorageProvider storageProvider) : IFileDialogService
{
    public async Task<string?> OpenOverlayAsync()
    {
        var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select overlay file",
            AllowMultiple = false
        });

        return files.FirstOrDefault()?.Path.LocalPath;
    }

    public async Task<string?> OpenFolderAsync()
    {
        var folders = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select folder",
            AllowMultiple = false
        });

        return folders.FirstOrDefault()?.Path.LocalPath;
    }

    public async Task<string?> OpenJsonlAsync()
    {
        var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select card JSONL file",
            AllowMultiple = false,
            FileTypeFilter =
            [new FilePickerFileType("JSONL files") { Patterns = ["*.jsonl", "*.json"] }]
        });

        return files.FirstOrDefault()?.Path.LocalPath;
    }
}
