using ProxyOverlay.Models;

namespace ProxyOverlay.Services;

public interface ICardDatabase
{
    CardRecord? FindByName(string cardName);
}
