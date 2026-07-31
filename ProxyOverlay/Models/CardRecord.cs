namespace ProxyOverlay.Models;

public sealed record CardRecord(
    string Name,
    string Layout,
    string TypeLine,
    string BorderColor,
    string Frame)
{
    // Kept for callers that provide a pre-computed overlay type (including the
    // existing view-model tests). Imported records derive this from their data.
    private readonly string? _overlayType;

    public CardRecord(string name, string overlayType)
        : this(name, string.Empty, string.Empty, string.Empty, string.Empty) =>
        _overlayType = overlayType;

    public string OverlayType => _overlayType ?? DeriveOverlayType();

    private string DeriveOverlayType()
    {
        var frame = Frame switch
        {
            "1993" or "1997" or "2003" => "Retro",
            "2015" => "Modern",
            _ => string.Empty
        };

        if (TypeLine.Contains("Creature", StringComparison.OrdinalIgnoreCase))
            frame += " Creature";
        else if (TypeLine.Contains("Planeswalker", StringComparison.OrdinalIgnoreCase))
            frame += " Planeswalker";

        return frame.Trim();
    }
}
