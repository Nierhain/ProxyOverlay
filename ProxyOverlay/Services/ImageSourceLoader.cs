using Avalonia.Platform;
using ImageMagick;

namespace ProxyOverlay.Services;

internal static class ImageSourceLoader
{
    public static MagickImage Load(string source)
    {
        if (!source.StartsWith("avares://", StringComparison.OrdinalIgnoreCase))
        {
            return new MagickImage(source);
        }

        using var stream = AssetLoader.Open(new Uri(source));
        return new MagickImage(stream);
    }
}
