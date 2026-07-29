namespace ProxyOverlay.Models;

public sealed record OverlayChoice(string Frame, string CardType, string FilePath)
{
    public string DisplayName =>
        CardType == "Spell" ? Frame : $"{Frame} {CardType}";
}
