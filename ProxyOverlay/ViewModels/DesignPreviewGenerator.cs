using Avalonia.Media.Imaging;
using ProxyOverlay.Services;

namespace ProxyOverlay.ViewModels;

internal sealed class DesignPreviewGenerator : IPreviewGenerator
{
    public Bitmap CreatePreview(string imagePath, string overlayPath, uint maxWidth, uint maxHeight)
    {
        return new Bitmap("");
    }
}
