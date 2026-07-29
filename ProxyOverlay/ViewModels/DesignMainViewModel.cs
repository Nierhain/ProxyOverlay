using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using ProxyOverlay.Services;

namespace ProxyOverlay.ViewModels;

public class DesignMainViewModel: MainViewModel 
{
    public DesignMainViewModel(): base(new DesignImageProcessor(), new DesignFilesService(), new DesignFileDialogService(), new DesignPreviewGenerator(), new EmptyCardDatabase())
    {
        using var stream = AssetLoader.Open(
            new Uri("avares://ProxyOverlay/Assets/designer_preview.png"));

        PreviewImage = new Bitmap(stream);
    }
}
