using Microsoft.Extensions.DependencyInjection;
using ProxyOverlay.Services;
using ProxyOverlay.ViewModels;

namespace ProxyOverlay;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection services)
    {
        services.AddSingleton<IImageProcessor, ImageProcessor>();
        services.AddSingleton<ICardDatabase>(_ => new JsonlCardDatabase(
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ProxyOverlay", "cards.sqlite")));
        services.AddSingleton<IFilesService, FilesService>();
        services.AddSingleton<IFileDialogService, AvaloniaFileDialogService>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<IPreviewGenerator, PreviewGenerator>();
    }
}
