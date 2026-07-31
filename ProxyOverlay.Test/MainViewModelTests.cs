using Avalonia.Media.Imaging;
using ProxyOverlay.Models;
using ProxyOverlay.Services;
using ProxyOverlay.ViewModels;

namespace ProxyOverlay.Test;

public sealed class MainViewModelTests
{
    [Fact]
    public async Task OtherPurposeMode_UsesChosenOverlayForEveryImage()
    {
        using var input = TestDirectory.Create();
        input.AddFile("Card One.png");
        input.AddFile("Card Two.jpg");

        var processor = new RecordingImageProcessor();
        var viewModel = CreateViewModel(
            processor: processor,
            fileDialogService: new FakeFileDialogService { NextFolder = input.Path });
        viewModel.ChosenFile = "all-images-overlay.png";
        viewModel.IsOtherPurpose = true;
        viewModel.InputFolder = input.Path;
        viewModel.OutputFolder = input.Path;

        await viewModel.OpenInputFolderCommand.ExecuteAsync(null);
        await viewModel.ProcessImagesCommand.ExecuteAsync(null);

        Assert.Equal(2, processor.OverlayFiles.Count);
        Assert.All(processor.OverlayFiles.Values,
            overlayFile => Assert.Equal("all-images-overlay.png", overlayFile));
    }

    [Fact]
    public async Task CardDatabaseMatching_SelectsOverlayFromCardNameAndType()
    {
        using var input = TestDirectory.Create();
        input.AddFile("Lightning Bolt.png");

        var database = new RecordingCardDatabase(
            new CardRecord("Lightning Bolt", "Retro Creature"));
        var viewModel = CreateViewModel(
            cardDatabase: database,
            fileDialogService: new FakeFileDialogService { NextFolder = input.Path });
        viewModel.UseCardDatabaseOverlayMatching = true;
        viewModel.RetroCreatureOverlayFile = "retro-creature.png";
        viewModel.InputFolder = input.Path;

        await viewModel.OpenInputFolderCommand.ExecuteAsync(null);

        var preview = Assert.Single(viewModel.ImagePreviews);
        Assert.Equal("Retro Creature", preview.SelectedOverlay.DisplayName);
        Assert.Equal("retro-creature.png", preview.SelectedOverlay.FilePath);
        Assert.Equal("Lightning Bolt", database.LastLookup);
    }

    [Fact]
    public async Task OverlayFolder_LoadsFilesIntoMatchingFrameAndTypeSlots()
    {
        using var overlays = TestDirectory.Create();
        overlays.AddFile("Modern_Spell.png");
        overlays.AddFile("Retro_Creature.webp");
        overlays.AddFile("M15_Spell.png");
        overlays.AddFile("not-an-overlay.txt");

        var dialogs = new FakeFileDialogService { NextFolder = overlays.Path };
        var viewModel = CreateViewModel(fileDialogService: dialogs);

        await viewModel.OpenOverlayFolderCommand.ExecuteAsync(null);

        Assert.Equal(overlays.Path, viewModel.OverlayFolder);
        Assert.Equal(Path.Combine(overlays.Path, "Modern_Spell.png"), viewModel.ModernOverlayFile);
        Assert.Equal(Path.Combine(overlays.Path, "Retro_Creature.webp"), viewModel.RetroCreatureOverlayFile);
        Assert.Equal(Path.Combine(overlays.Path, "M15_Spell.png"), viewModel.M15OverlayFile);
        Assert.Equal(1, dialogs.LastFolderLoadCount);
    }

    [Fact]
    public async Task CardDatabaseMatching_UsesM15SpellWhenCardIsNotFound()
    {
        using var input = TestDirectory.Create();
        input.AddFile("Unknown Card.png");

        var viewModel = CreateViewModel(
            cardDatabase: new RecordingCardDatabase(null),
            fileDialogService: new FakeFileDialogService { NextFolder = input.Path });
        viewModel.UseCardDatabaseOverlayMatching = true;
        viewModel.InputFolder = input.Path;

        await viewModel.OpenInputFolderCommand.ExecuteAsync(null);

        var preview = Assert.Single(viewModel.ImagePreviews);
        Assert.Equal("M15", preview.SelectedOverlay.DisplayName);
        Assert.Equal(viewModel.M15OverlayFile, preview.SelectedOverlay.FilePath);
    }

    [Fact]
    public async Task SelectingInputFolderWithoutOverlayDoesNotGeneratePreviewWithEmptyPath()
    {
        using var input = TestDirectory.Create();
        input.AddFile("Card.png");

        var previewGenerator = new RejectsEmptyOverlayPreviewGenerator();
        var viewModel = CreateViewModel(
            previewGenerator: previewGenerator,
            fileDialogService: new FakeFileDialogService { NextFolder = input.Path });

        await viewModel.OpenInputFolderCommand.ExecuteAsync(null);

        Assert.Single(viewModel.ImagePreviews);
        Assert.Equal("Select an overlay to generate previews.", viewModel.Status);
        Assert.Equal(0, previewGenerator.CallsWithEmptyOverlay);
    }

    private static MainViewModel CreateViewModel(
        RecordingImageProcessor? processor = null,
        ICardDatabase? cardDatabase = null,
        IFileDialogService? fileDialogService = null,
        IPreviewGenerator? previewGenerator = null)
    {
        return new MainViewModel(
            processor ?? new RecordingImageProcessor(),
            new FakeFilesService(),
            fileDialogService ?? new FakeFileDialogService(),
            previewGenerator ?? new FakePreviewGenerator(),
            cardDatabase ?? new RecordingCardDatabase(null));
    }

    private sealed class RecordingImageProcessor : IImageProcessor
    {
        public Dictionary<string, string> OverlayFiles { get; } = new(StringComparer.OrdinalIgnoreCase);

        public Task ProcessAsync(
            string inputFolder,
            string outputFolder,
            IReadOnlyDictionary<string, string> overlayFiles,
            double overlayPercent,
            IProgress<ProcessProgress> progress,
            CancellationToken token = default)
        {
            OverlayFiles.Clear();
            foreach (var pair in overlayFiles)
            {
                OverlayFiles[pair.Key] = pair.Value;
            }

            return Task.CompletedTask;
        }
    }

    private sealed class RecordingCardDatabase(CardRecord? card) : ICardDatabase
    {
        public string? LastLookup { get; private set; }

        public CardRecord? FindByName(string cardName)
        {
            LastLookup = cardName;
            return card is not null &&
                   string.Equals(card.Name, cardName, StringComparison.OrdinalIgnoreCase)
                ? card
                : null;
        }
    }

    private sealed class FakePreviewGenerator : IPreviewGenerator
    {
        public Bitmap CreatePreview(string imagePath, string overlayPath, uint maxWidth, uint maxHeight) => null!;
    }

    private sealed class RejectsEmptyOverlayPreviewGenerator : IPreviewGenerator
    {
        public int CallsWithEmptyOverlay { get; private set; }

        public Bitmap CreatePreview(string imagePath, string overlayPath, uint maxWidth, uint maxHeight)
        {
            if (string.IsNullOrWhiteSpace(overlayPath))
                CallsWithEmptyOverlay++;

            return null!;
        }
    }

    private sealed class FakeFilesService : IFilesService
    {
        public Task<string?> LoadFirstFile(string path) => Task.FromResult<string?>(null);
    }

    private sealed class FakeFileDialogService : IFileDialogService
    {
        public string? NextFolder { get; init; }
        public int LastFolderLoadCount { get; private set; }

        public Task<string?> OpenOverlayAsync() => Task.FromResult<string?>(null);

        public Task<string?> OpenFolderAsync()
        {
            LastFolderLoadCount++;
            return Task.FromResult(NextFolder);
        }

        public Task<string?> OpenJsonlAsync() => Task.FromResult<string?>(null);
    }

    private sealed class TestDirectory : IDisposable
    {
        private TestDirectory(string path) => Path = path;

        public string Path { get; }

        public static TestDirectory Create()
        {
            var path = System.IO.Path.Combine(
                System.IO.Path.GetTempPath(),
                "ProxyOverlayTests",
                Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(path);
            return new TestDirectory(path);
        }

        public void AddFile(string fileName) => File.WriteAllText(System.IO.Path.Combine(Path, fileName), string.Empty);

        public void Dispose()
        {
            if (Directory.Exists(Path))
            {
                Directory.Delete(Path, recursive: true);
            }
        }
    }
}
