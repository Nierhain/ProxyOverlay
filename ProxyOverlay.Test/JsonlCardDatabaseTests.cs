using ProxyOverlay.Services;

namespace ProxyOverlay.Test;

public sealed class JsonlCardDatabaseTests
{
    [Fact]
    public async Task ImportAsync_StoresAndFindsSelectedFields()
    {
        using var directory = TemporaryDirectory.Create();
        var jsonl = Path.Combine(directory.Path, "cards.jsonl");
        await File.WriteAllTextAsync(jsonl, """
            {"name":"Nissa, Worldsoul Speaker","layout":"normal","type_line":"Legendary Creature — Elf Druid","border_color":"black","frame":"2015","extra":"ignored"}
            {"name":"Lightning Bolt","layout":"normal","type_line":"Instant","border_color":"black","frame":"1993"}
            """);

        var database = new JsonlCardDatabase(Path.Combine(directory.Path, "cards.sqlite"));

        Assert.Equal(2, await database.ImportAsync(jsonl));
        var card = database.FindByName("nissa, worldsoul speaker");

        Assert.NotNull(card);
        Assert.Equal("Nissa, Worldsoul Speaker", card.Name);
        Assert.Equal("normal", card.Layout);
        Assert.Equal("Legendary Creature — Elf Druid", card.TypeLine);
        Assert.Equal("black", card.BorderColor);
        Assert.Equal("2015", card.Frame);
        Assert.Equal("Modern Creature", card.OverlayType);
    }

    private sealed class TemporaryDirectory : IDisposable
    {
        private TemporaryDirectory(string path) => Path = path;
        public string Path { get; }

        public static TemporaryDirectory Create()
        {
            var path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "ProxyOverlayTests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(path);
            return new TemporaryDirectory(path);
        }

        public void Dispose()
        {
            if (Directory.Exists(Path)) Directory.Delete(Path, recursive: true);
        }
    }
}
