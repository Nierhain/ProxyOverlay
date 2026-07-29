using ProxyOverlay.Services;

namespace ProxyOverlay.ViewModels;

internal sealed class DesignFilesService : IFilesService
{
    public Task<string?> LoadFirstFile(string path)
    {
        return Task.FromResult<string?>(null);
    }
}
