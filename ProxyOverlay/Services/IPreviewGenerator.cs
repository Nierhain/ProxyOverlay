using Avalonia.Media.Imaging;

namespace ProxyOverlay.Services;

public interface IPreviewGenerator
{
    Bitmap CreatePreview(string imagePath, string overlayPath, uint maxWidth, uint maxHeight);
}
