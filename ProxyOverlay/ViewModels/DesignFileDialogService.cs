using ProxyOverlay.Services;

namespace ProxyOverlay.ViewModels;

internal sealed class DesignFileDialogService : IFileDialogService
{
    public Task<string?> OpenOverlayAsync() => Task.FromResult<string?>(null);
    public Task<string?> OpenFolderAsync() => Task.FromResult<string?>(null);
    public Task<string?> OpenJsonlAsync() => Task.FromResult<string?>(null);
}
