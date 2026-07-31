namespace ProxyOverlay.Services;

public interface IFileDialogService
{
    Task<string?> OpenOverlayAsync();
    Task<string?> OpenFolderAsync();
    Task<string?> OpenJsonlAsync();
}
