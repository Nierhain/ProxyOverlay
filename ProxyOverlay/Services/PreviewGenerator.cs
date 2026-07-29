using Avalonia.Media.Imaging;
using ImageMagick;

namespace ProxyOverlay.Services;

public interface IPreviewGenerator
{
    Bitmap CreatePreview(string imagePath, string overlayPath, uint maxWidth, uint maxHeight);
}
public class PreviewGenerator: IPreviewGenerator
{
    public Bitmap CreatePreview(string imagePath, string overlayPath, uint maxWidth, uint maxHeight)
    {
        using var image = ImageSourceLoader.Load(imagePath);
        using var overlay = ImageSourceLoader.Load(overlayPath);

        // The preview is never displayed larger than this, so avoid decoding and
        // compositing full-resolution source images when they are much larger.
        image.AutoOrient();
        image.Resize(new MagickGeometry(maxWidth, maxHeight));

        overlay.Resize(image.Width, 0);
        image.Composite(overlay, Gravity.South, CompositeOperator.Over);

        var stream = new MemoryStream();
        image.Write(stream, MagickFormat.Png);
        stream.Position = 0;

        var bitmap = new Bitmap(stream);
        stream.Dispose();
        return bitmap;
    }
}
