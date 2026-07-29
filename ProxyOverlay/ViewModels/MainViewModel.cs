using System;
using System.IO;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProxyOverlay.Models;
using ProxyOverlay.Services;

namespace ProxyOverlay.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(InputFolderDisplay))]
    [NotifyCanExecuteChangedFor(nameof(ProcessImagesCommand))]
    public partial string InputFolder { get; set; }
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(OutputFolderDisplay))]
    [NotifyCanExecuteChangedFor(nameof(ProcessImagesCommand))]
    public partial string OutputFolder { get; set; }
    [ObservableProperty]
    public partial string OverlayFolder { get; set; }

    [ObservableProperty]
    public partial bool UseCardDatabaseOverlayMatching { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowMagicProxySettings))]
    public partial bool IsOtherPurpose { get; set; }

    public bool ShowMagicProxySettings => !IsOtherPurpose;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ChosenFileDisplay))]
    public partial string ChosenFile { get; set; }
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ModernOverlayFileDisplay))]
    public partial string ModernOverlayFile { get; set; } = string.Empty;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ModernCreatureOverlayFileDisplay))]
    public partial string ModernCreatureOverlayFile { get; set; } = string.Empty;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ModernPlaneswalkerOverlayFileDisplay))]
    public partial string ModernPlaneswalkerOverlayFile { get; set; } = string.Empty;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RetroOverlayFileDisplay))]
    public partial string RetroOverlayFile { get; set; } = string.Empty;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RetroCreatureOverlayFileDisplay))]
    public partial string RetroCreatureOverlayFile { get; set; } = string.Empty;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(M15OverlayFileDisplay))]
    public partial string M15OverlayFile { get; set; } = string.Empty;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(M15ClassOverlayFileDisplay))]
    public partial string M15ClassOverlayFile { get; set; } = string.Empty;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(M15CreatureOverlayFileDisplay))]
    public partial string M15CreatureOverlayFile { get; set; } = string.Empty;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(M15PlaneswalkerOverlayFileDisplay))]
    public partial string M15PlaneswalkerOverlayFile { get; set; } = string.Empty;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(M15RoomOverlayFileDisplay))]
    public partial string M15RoomOverlayFile { get; set; } = string.Empty;

    partial void OnModernOverlayFileChanged(string value) => UpdateOverlayChoices();
    partial void OnModernCreatureOverlayFileChanged(string value) => UpdateOverlayChoices();
    partial void OnModernPlaneswalkerOverlayFileChanged(string value) => UpdateOverlayChoices();
    partial void OnRetroOverlayFileChanged(string value) => UpdateOverlayChoices();
    partial void OnRetroCreatureOverlayFileChanged(string value) => UpdateOverlayChoices();
    partial void OnM15OverlayFileChanged(string value) => UpdateOverlayChoices();
    partial void OnM15ClassOverlayFileChanged(string value) => UpdateOverlayChoices();
    partial void OnM15CreatureOverlayFileChanged(string value) => UpdateOverlayChoices();
    partial void OnM15PlaneswalkerOverlayFileChanged(string value) => UpdateOverlayChoices();
    partial void OnM15RoomOverlayFileChanged(string value) => UpdateOverlayChoices();
    partial void OnIsOtherPurposeChanged(bool value) => UpdateOverlayChoices();

    partial void OnUseCardDatabaseOverlayMatchingChanged(bool value)
    {
        UpdateOverlayChoices();
    }
    
    public string InputFolderDisplay => InputFolder ?? string.Empty;
    public string OutputFolderDisplay => OutputFolder ?? string.Empty;
    public string ChosenFileDisplay => DisplayPath(ChosenFile);
    public string ModernOverlayFileDisplay => DisplayPath(ModernOverlayFile);
    public string ModernCreatureOverlayFileDisplay => DisplayPath(ModernCreatureOverlayFile);
    public string ModernPlaneswalkerOverlayFileDisplay => DisplayPath(ModernPlaneswalkerOverlayFile);
    public string RetroOverlayFileDisplay => DisplayPath(RetroOverlayFile);
    public string RetroCreatureOverlayFileDisplay => DisplayPath(RetroCreatureOverlayFile);
    public string M15OverlayFileDisplay => DisplayPath(M15OverlayFile);
    public string M15ClassOverlayFileDisplay => DisplayPath(M15ClassOverlayFile);
    public string M15CreatureOverlayFileDisplay => DisplayPath(M15CreatureOverlayFile);
    public string M15PlaneswalkerOverlayFileDisplay => DisplayPath(M15PlaneswalkerOverlayFile);
    public string M15RoomOverlayFileDisplay => DisplayPath(M15RoomOverlayFile);

    private static string DisplayPath(string path) =>
        string.Equals(path, Defaults.DefaultOverlayFile, StringComparison.Ordinal)
            ? string.Empty
            : path;

    public ObservableCollection<ImageOverlayPreview> ImagePreviews { get; } = [];
    
    [ObservableProperty]
    public partial Bitmap? PreviewImage { get; set; }
    [ObservableProperty]
    public partial bool ShowOverlay { get; set; } = true;
    [ObservableProperty]
    public partial double OverlayScale { get; set; } = 100;
    [ObservableProperty]
    private partial int CurrentFile { get; set; }

    [ObservableProperty] 
    public partial double Progress { get; set; }
    [ObservableProperty]
    private partial int FilesCount { get; set; }
    
    [ObservableProperty]
    public partial string Status { get; set; }
    [ObservableProperty] public partial bool IsProcessing { get; set; }
    [ObservableProperty] public partial bool IsPreviewLoading { get; set; }
    
    
    private readonly IProgress<ProcessProgress> _progress;
    private readonly IFilesService _filesService;
    private readonly IFileDialogService _fileDialogService;
    private readonly IImageProcessor _imageProcessor;
    private readonly IPreviewGenerator _previewGenerator;
    private readonly ICardDatabase _cardDatabase;
    private CancellationTokenSource? _previewCancellation;

    private const uint PreviewWidth = 644;
    private const uint PreviewHeight = 900;

    [RelayCommand]
    private async Task OpenInputFolder()
    {
        var folder = await _fileDialogService.OpenFolderAsync();
        if (folder is null) return;
        
        InputFolder = folder;
        await LoadImagePreviews();
    }
    
    [RelayCommand]
    private async Task OpenOutputFolder()
    {
        var folder = await _fileDialogService.OpenFolderAsync();
        if (folder is null) return;
        
        OutputFolder = folder;
    }
    
    [RelayCommand]
    private async Task OpenOverlayFile()
    {
        var file = await _fileDialogService.OpenOverlayAsync();
        if(file is null) return;
        
        ChosenFile = file;
        UpdateOverlayChoices();
    }

    [RelayCommand]
    private async Task OpenOverlayFolder()
    {
        var folder = await _fileDialogService.OpenFolderAsync();
        if (folder is null) return;

        OverlayFolder = folder;
        LoadOverlayFiles(folder);
    }

    private void LoadOverlayFiles(string folder)
    {
        ResetOverlayFiles();

        var loaded = 0;
        foreach (var file in Directory.EnumerateFiles(folder).Where(IsImage))
        {
            var name = Path.GetFileNameWithoutExtension(file)
                .Split('_', 2, StringSplitOptions.TrimEntries);
            var frame = name[0];
            var cardType = name.Length == 2 ? name[1] : string.Empty;

            if (AssignOverlayFile(frame, cardType, file))
            {
                loaded++;
            }
        }

        UpdateOverlayChoices();
        Status = $"Loaded {loaded} overlay file{(loaded == 1 ? "" : "s")}";
    }

    private void ResetOverlayFiles()
    {
        ChosenFile = Defaults.DefaultOverlayFile;
        ModernOverlayFile = string.Empty;
        ModernCreatureOverlayFile = string.Empty;
        ModernPlaneswalkerOverlayFile = string.Empty;
        RetroOverlayFile = string.Empty;
        RetroCreatureOverlayFile = string.Empty;
        M15OverlayFile = string.Empty;
        M15ClassOverlayFile = string.Empty;
        M15CreatureOverlayFile = string.Empty;
        M15PlaneswalkerOverlayFile = string.Empty;
        M15RoomOverlayFile = string.Empty;
    }

    private bool AssignOverlayFile(string frame, string cardType, string file)
    {
        var normalizedFrame = NormalizeFilePart(frame);
        var normalizedCardType = NormalizeFilePart(cardType);
        var isDefaultType = string.IsNullOrEmpty(normalizedCardType) ||
                            normalizedCardType is "base" or "default";

        if (normalizedFrame == "default" && isDefaultType)
        {
            ChosenFile = file;
            return true;
        }

        if (normalizedFrame is not ("modern" or "retro" or "m15")) return false;

        switch (normalizedFrame, normalizedCardType)
        {
            case ("modern", "spell"): ModernOverlayFile = file; break;
            case ("modern", "creature"): ModernCreatureOverlayFile = file; break;
            case ("modern", "planeswalker"): ModernPlaneswalkerOverlayFile = file; break;
            case ("retro", "spell"): RetroOverlayFile = file; break;
            case ("retro", "creature"): RetroCreatureOverlayFile = file; break;
            case ("m15", "spell"): M15OverlayFile = file; break;
            case ("m15", "class"): M15ClassOverlayFile = file; break;
            case ("m15", "creature"): M15CreatureOverlayFile = file; break;
            case ("m15", "planeswalker"): M15PlaneswalkerOverlayFile = file; break;
            case ("m15", "room"): M15RoomOverlayFile = file; break;
            default: return false;
        }

        return true;
    }

    private static string NormalizeFilePart(string value) =>
        value.Replace(" ", string.Empty)
            .Replace("-", string.Empty)
            .ToLowerInvariant();

    [RelayCommand]
    private async Task OpenModernOverlayFile() => await SelectOverlayFile(file => ModernOverlayFile = file);

    [RelayCommand]
    private async Task OpenModernCreatureOverlayFile() => await SelectOverlayFile(file => ModernCreatureOverlayFile = file);

    [RelayCommand]
    private async Task OpenModernPlaneswalkerOverlayFile() => await SelectOverlayFile(file => ModernPlaneswalkerOverlayFile = file);

    [RelayCommand]
    private async Task OpenRetroOverlayFile() => await SelectOverlayFile(file => RetroOverlayFile = file);

    [RelayCommand]
    private async Task OpenRetroCreatureOverlayFile() => await SelectOverlayFile(file => RetroCreatureOverlayFile = file);

    [RelayCommand]
    private async Task OpenM15OverlayFile() => await SelectOverlayFile(file => M15OverlayFile = file);

    [RelayCommand]
    private async Task OpenM15ClassOverlayFile() => await SelectOverlayFile(file => M15ClassOverlayFile = file);

    [RelayCommand]
    private async Task OpenM15CreatureOverlayFile() => await SelectOverlayFile(file => M15CreatureOverlayFile = file);

    [RelayCommand]
    private async Task OpenM15PlaneswalkerOverlayFile() => await SelectOverlayFile(file => M15PlaneswalkerOverlayFile = file);

    [RelayCommand]
    private async Task OpenM15RoomOverlayFile() => await SelectOverlayFile(file => M15RoomOverlayFile = file);

    private async Task SelectOverlayFile(Action<string> assign)
    {
        var file = await _fileDialogService.OpenOverlayAsync();
        if (file is null) return;

        assign(file);
        UpdateOverlayChoices();
    }

    private void UpdateOverlayChoices()
    {
        var choices = GetOverlayChoices();
        foreach (var preview in ImagePreviews)
        {
            var preferredChoice = GetAutomaticOverlayChoice(preview.FilePath, choices);
            preview.UpdateOverlayChoices(choices, preferredChoice);
        }
    }

    private OverlayChoice? GetAutomaticOverlayChoice(
        string imagePath,
        IReadOnlyList<OverlayChoice> choices)
    {
        if (!UseCardDatabaseOverlayMatching || IsOtherPurpose) return null;

        var cardName = Path.GetFileNameWithoutExtension(imagePath);
        var card = _cardDatabase.FindByName(cardName);
        if (card is null || string.IsNullOrWhiteSpace(card.OverlayType)) return choices[0];

        var overlayType = NormalizeFilePart(card.OverlayType);
        return choices.FirstOrDefault(choice =>
                   NormalizeFilePart(choice.DisplayName) == overlayType)
               ?? choices[0];
    }

    private bool CanProcessImages() =>
        !string.IsNullOrWhiteSpace(InputFolder) &&
        !string.IsNullOrWhiteSpace(OutputFolder);

    [RelayCommand(CanExecute = nameof(CanProcessImages))]
    private async Task ProcessImages()
    {
        IsProcessing = true;
        try
        {
            var overlayFiles = ImagePreviews.ToDictionary(
                preview => preview.FilePath,
                preview => IsOtherPurpose ? ChosenFile : preview.SelectedOverlay.FilePath,
                StringComparer.OrdinalIgnoreCase);
            await Task.Run(() => _imageProcessor.ProcessAsync(
                InputFolder, OutputFolder, overlayFiles, OverlayScale, _progress));
        }
        finally
        {
            IsProcessing = false;
        }
    }

    private IReadOnlyList<OverlayChoice> GetOverlayChoices()
    {
        if (IsOtherPurpose)
        {
            return [new("Overlay", "Spell", ChosenFile)];
        }

        return
        [
            new("M15", "Spell", M15OverlayFile),
            new("Modern", "Spell", ModernOverlayFile),
            new("Modern", "Creature", ModernCreatureOverlayFile),
            new("Modern", "Planeswalker", ModernPlaneswalkerOverlayFile),
            new("Retro", "Spell", RetroOverlayFile),
            new("Retro", "Creature", RetroCreatureOverlayFile),
            new("M15", "Class", M15ClassOverlayFile),
            new("M15", "Creature", M15CreatureOverlayFile),
            new("M15", "Planeswalker", M15PlaneswalkerOverlayFile),
            new("M15", "Room", M15RoomOverlayFile)
        ];
    }

    private async Task LoadImagePreviews()
    {
        _previewCancellation?.Cancel();
        _previewCancellation?.Dispose();
        var cancellation = _previewCancellation = new CancellationTokenSource();
        var token = cancellation.Token;

        IsPreviewLoading = true;
        Status = "Generating preview...";

        try
        {
            var files = Directory.EnumerateFiles(InputFolder).Where(IsImage).ToList();
            var choices = GetOverlayChoices();
            var previews = files.Select(file =>
            {
                var selected = GetAutomaticOverlayChoice(file, choices) ?? choices[0];
                var item = new ImageOverlayPreview(file, choices, selected);
                item.OverlayChanged += OnOverlayChanged;
                return item;
            }).ToList();

            ImagePreviews.Clear();
            foreach (var preview in previews)
            {
                ImagePreviews.Add(preview);
                preview.Preview = await Task.Run(
                    () => _previewGenerator.CreatePreview(preview.FilePath,
                        preview.SelectedOverlay.FilePath, 220, 308), token);
                token.ThrowIfCancellationRequested();
            }
            FilesCount = previews.Count;
            token.ThrowIfCancellationRequested();
            Status = "Ready";
        }
        catch (OperationCanceledException)
        {
            // A newer preview request has superseded this one.
        }
        finally
        {
            if (ReferenceEquals(_previewCancellation, cancellation))
            {
                IsPreviewLoading = false;
            }
        }
    }

    private async void OnOverlayChanged(object? sender, EventArgs e)
    {
        if (sender is not ImageOverlayPreview preview) return;
        if (preview.SelectedOverlay is not { } selectedOverlay) return;

        // Capture the paths before starting the background task. Changing mode or
        // refreshing the choices can temporarily clear SelectedOverlay.
        var imagePath = preview.FilePath;
        var overlayPath = selectedOverlay.FilePath;

        try
        {
            preview.Preview = await Task.Run(
                () => _previewGenerator.CreatePreview(imagePath, overlayPath, 220, 308));
        }
        catch (Exception exception) when (exception is IOException or InvalidOperationException)
        {
            Status = $"Could not generate preview: {exception.Message}";
        }
    }

    private static readonly HashSet<string> ImageExtensions =
    [".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp", ".tif", ".tiff"];

    private static bool IsImage(string file) =>
        ImageExtensions.Contains(Path.GetExtension(file).ToLowerInvariant());

    public void Initialize()
    {
        PreviewImage = _previewGenerator.CreatePreview(Defaults.DefaultPreviewImage, ChosenFile, PreviewWidth, PreviewHeight);
    }
    
    public MainViewModel(
        IImageProcessor imageProcessor,
        IFilesService filesService,
        IFileDialogService fileDialogService,
        IPreviewGenerator previewGenerator,
        ICardDatabase cardDatabase)
    {
        _imageProcessor = imageProcessor;
        _filesService = filesService;
        _fileDialogService = fileDialogService;
        _previewGenerator = previewGenerator;
        _cardDatabase = cardDatabase;
        _progress = new Progress<ProcessProgress>(current =>
        {
            CurrentFile = current.Current;
            FilesCount = current.Total;
            Progress = (double)CurrentFile / FilesCount * 100;
            Status = CurrentFile == FilesCount ? "Processing finished" : $"Processing {CurrentFile}/{FilesCount}";
        });
        Status = "Ready";        
        ChosenFile = Defaults.DefaultOverlayFile;
    }
}
