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
