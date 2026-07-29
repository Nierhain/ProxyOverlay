using Avalonia.Platform.Storage;

namespace ProxyOverlay.Services;

public sealed class FilesService : IFilesService
{
    public Task<string?> LoadFirstFile(string path)
    {
        var file = Directory.EnumerateFiles(path).FirstOrDefault();
        return Task.FromResult(file);
    }
}

public interface IFilesService
{
    Task<string?> LoadFirstFile(string path);
}

public interface IFileDialogService
{
    Task<string?> OpenOverlayAsync();
    Task<string?> OpenFolderAsync();
}

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
}
