using ProxyOverlay.Models;

namespace ProxyOverlay.Services;

// Placeholder until the card data source is implemented.
public sealed class EmptyCardDatabase : ICardDatabase
{
    public CardRecord? FindByName(string cardName) => null;
}
