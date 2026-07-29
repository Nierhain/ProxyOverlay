namespace ProxyOverlay.Services;

public interface IFilesService
{
    Task<string?> LoadFirstFile(string path);
}
