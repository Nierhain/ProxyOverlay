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
using ProxyOverlay.Services;

namespace ProxyOverlay.ViewModels;

public record ProcessProgress(int Current, int Total);

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
    [NotifyPropertyChangedFor(nameof(ChosenFileDisplay))]
    public partial string ChosenFile { get; set; }
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ModernOverlayFileDisplay))]
    public partial string ModernOverlayFile { get; set; } = Defaults.DefaultOverlayFile;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ModernClassOverlayFileDisplay))]
    public partial string ModernClassOverlayFile { get; set; } = Defaults.DefaultOverlayFile;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ModernCreatureOverlayFileDisplay))]
    public partial string ModernCreatureOverlayFile { get; set; } = Defaults.DefaultOverlayFile;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ModernPlaneswalkerOverlayFileDisplay))]
    public partial string ModernPlaneswalkerOverlayFile { get; set; } = Defaults.DefaultOverlayFile;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ModernRoomOverlayFileDisplay))]
    public partial string ModernRoomOverlayFile { get; set; } = Defaults.DefaultOverlayFile;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RetroOverlayFileDisplay))]
    public partial string RetroOverlayFile { get; set; } = Defaults.DefaultOverlayFile;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RetroClassOverlayFileDisplay))]
    public partial string RetroClassOverlayFile { get; set; } = Defaults.DefaultOverlayFile;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RetroCreatureOverlayFileDisplay))]
    public partial string RetroCreatureOverlayFile { get; set; } = Defaults.DefaultOverlayFile;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RetroPlaneswalkerOverlayFileDisplay))]
    public partial string RetroPlaneswalkerOverlayFile { get; set; } = Defaults.DefaultOverlayFile;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RetroRoomOverlayFileDisplay))]
    public partial string RetroRoomOverlayFile { get; set; } = Defaults.DefaultOverlayFile;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(M15OverlayFileDisplay))]
    public partial string M15OverlayFile { get; set; } = Defaults.DefaultOverlayFile;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(M15ClassOverlayFileDisplay))]
    public partial string M15ClassOverlayFile { get; set; } = Defaults.DefaultOverlayFile;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(M15CreatureOverlayFileDisplay))]
    public partial string M15CreatureOverlayFile { get; set; } = Defaults.DefaultOverlayFile;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(M15PlaneswalkerOverlayFileDisplay))]
    public partial string M15PlaneswalkerOverlayFile { get; set; } = Defaults.DefaultOverlayFile;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(M15RoomOverlayFileDisplay))]
    public partial string M15RoomOverlayFile { get; set; } = Defaults.DefaultOverlayFile;

    partial void OnModernOverlayFileChanged(string value) => UpdateOverlayChoices();
    partial void OnModernClassOverlayFileChanged(string value) => UpdateOverlayChoices();
    partial void OnModernCreatureOverlayFileChanged(string value) => UpdateOverlayChoices();
    partial void OnModernPlaneswalkerOverlayFileChanged(string value) => UpdateOverlayChoices();
    partial void OnModernRoomOverlayFileChanged(string value) => UpdateOverlayChoices();
    partial void OnRetroOverlayFileChanged(string value) => UpdateOverlayChoices();
    partial void OnRetroClassOverlayFileChanged(string value) => UpdateOverlayChoices();
    partial void OnRetroCreatureOverlayFileChanged(string value) => UpdateOverlayChoices();
    partial void OnRetroPlaneswalkerOverlayFileChanged(string value) => UpdateOverlayChoices();
    partial void OnRetroRoomOverlayFileChanged(string value) => UpdateOverlayChoices();
    partial void OnM15OverlayFileChanged(string value) => UpdateOverlayChoices();
    partial void OnM15ClassOverlayFileChanged(string value) => UpdateOverlayChoices();
    partial void OnM15CreatureOverlayFileChanged(string value) => UpdateOverlayChoices();
    partial void OnM15PlaneswalkerOverlayFileChanged(string value) => UpdateOverlayChoices();
    partial void OnM15RoomOverlayFileChanged(string value) => UpdateOverlayChoices();
    
    public string InputFolderDisplay => InputFolder ?? string.Empty;
    public string OutputFolderDisplay => OutputFolder ?? string.Empty;
    public string ChosenFileDisplay => DisplayPath(ChosenFile);
    public string ModernOverlayFileDisplay => DisplayPath(ModernOverlayFile);
    public string ModernClassOverlayFileDisplay => DisplayPath(ModernClassOverlayFile);
    public string ModernCreatureOverlayFileDisplay => DisplayPath(ModernCreatureOverlayFile);
    public string ModernPlaneswalkerOverlayFileDisplay => DisplayPath(ModernPlaneswalkerOverlayFile);
    public string ModernRoomOverlayFileDisplay => DisplayPath(ModernRoomOverlayFile);
    public string RetroOverlayFileDisplay => DisplayPath(RetroOverlayFile);
    public string RetroClassOverlayFileDisplay => DisplayPath(RetroClassOverlayFile);
    public string RetroCreatureOverlayFileDisplay => DisplayPath(RetroCreatureOverlayFile);
    public string RetroPlaneswalkerOverlayFileDisplay => DisplayPath(RetroPlaneswalkerOverlayFile);
    public string RetroRoomOverlayFileDisplay => DisplayPath(RetroRoomOverlayFile);
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
        ModernOverlayFile = Defaults.DefaultOverlayFile;
        ModernClassOverlayFile = Defaults.DefaultOverlayFile;
        ModernCreatureOverlayFile = Defaults.DefaultOverlayFile;
        ModernPlaneswalkerOverlayFile = Defaults.DefaultOverlayFile;
        ModernRoomOverlayFile = Defaults.DefaultOverlayFile;
        RetroOverlayFile = Defaults.DefaultOverlayFile;
        RetroClassOverlayFile = Defaults.DefaultOverlayFile;
        RetroCreatureOverlayFile = Defaults.DefaultOverlayFile;
        RetroPlaneswalkerOverlayFile = Defaults.DefaultOverlayFile;
        RetroRoomOverlayFile = Defaults.DefaultOverlayFile;
        M15OverlayFile = Defaults.DefaultOverlayFile;
        M15ClassOverlayFile = Defaults.DefaultOverlayFile;
        M15CreatureOverlayFile = Defaults.DefaultOverlayFile;
        M15PlaneswalkerOverlayFile = Defaults.DefaultOverlayFile;
        M15RoomOverlayFile = Defaults.DefaultOverlayFile;
    }

    private bool AssignOverlayFile(string frame, string cardType, string file)
    {
        var normalizedFrame = NormalizeFilePart(frame);
        var normalizedCardType = NormalizeFilePart(cardType);
        var isBaseType = string.IsNullOrEmpty(normalizedCardType) ||
                         normalizedCardType is "base" or "default";

        if (normalizedFrame == "default" && isBaseType)
        {
            ChosenFile = file;
            return true;
        }

        if (normalizedFrame is not ("modern" or "retro" or "m15")) return false;

        switch (normalizedFrame, isBaseType ? "base" : normalizedCardType)
        {
            case ("modern", "base"): ModernOverlayFile = file; break;
            case ("modern", "class"): ModernClassOverlayFile = file; break;
            case ("modern", "creature"): ModernCreatureOverlayFile = file; break;
            case ("modern", "planeswalker"): ModernPlaneswalkerOverlayFile = file; break;
            case ("modern", "room"): ModernRoomOverlayFile = file; break;
            case ("retro", "base"): RetroOverlayFile = file; break;
            case ("retro", "class"): RetroClassOverlayFile = file; break;
            case ("retro", "creature"): RetroCreatureOverlayFile = file; break;
            case ("retro", "planeswalker"): RetroPlaneswalkerOverlayFile = file; break;
            case ("retro", "room"): RetroRoomOverlayFile = file; break;
            case ("m15", "base"): M15OverlayFile = file; break;
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
    private async Task OpenModernClassOverlayFile() => await SelectOverlayFile(file => ModernClassOverlayFile = file);

    [RelayCommand]
    private async Task OpenModernCreatureOverlayFile() => await SelectOverlayFile(file => ModernCreatureOverlayFile = file);

    [RelayCommand]
    private async Task OpenModernPlaneswalkerOverlayFile() => await SelectOverlayFile(file => ModernPlaneswalkerOverlayFile = file);

    [RelayCommand]
    private async Task OpenModernRoomOverlayFile() => await SelectOverlayFile(file => ModernRoomOverlayFile = file);

    [RelayCommand]
    private async Task OpenRetroOverlayFile() => await SelectOverlayFile(file => RetroOverlayFile = file);

    [RelayCommand]
    private async Task OpenRetroClassOverlayFile() => await SelectOverlayFile(file => RetroClassOverlayFile = file);

    [RelayCommand]
    private async Task OpenRetroCreatureOverlayFile() => await SelectOverlayFile(file => RetroCreatureOverlayFile = file);

    [RelayCommand]
    private async Task OpenRetroPlaneswalkerOverlayFile() => await SelectOverlayFile(file => RetroPlaneswalkerOverlayFile = file);

    [RelayCommand]
    private async Task OpenRetroRoomOverlayFile() => await SelectOverlayFile(file => RetroRoomOverlayFile = file);

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
            preview.UpdateOverlayChoices(choices);
        }
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
                preview => preview.SelectedOverlay.FilePath,
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
        return
        [
            new("Default", ChosenFile),
            new("Modern", ModernOverlayFile),
            new("Modern Class", ModernClassOverlayFile),
            new("Modern Creature", ModernCreatureOverlayFile),
            new("Modern Planeswalker", ModernPlaneswalkerOverlayFile),
            new("Modern Room", ModernRoomOverlayFile),
            new("Retro", RetroOverlayFile),
            new("Retro Class", RetroClassOverlayFile),
            new("Retro Creature", RetroCreatureOverlayFile),
            new("Retro Planeswalker", RetroPlaneswalkerOverlayFile),
            new("Retro Room", RetroRoomOverlayFile),
            new("M15", M15OverlayFile),
            new("M15 Class", M15ClassOverlayFile),
            new("M15 Creature", M15CreatureOverlayFile),
            new("M15 Planeswalker", M15PlaneswalkerOverlayFile),
            new("M15 Room", M15RoomOverlayFile)
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
                var selected = choices[0];
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

        try
        {
            preview.Preview = await Task.Run(
                () => _previewGenerator.CreatePreview(preview.FilePath,
                    preview.SelectedOverlay.FilePath, 220, 308));
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
        IPreviewGenerator previewGenerator)
    {
        _imageProcessor = imageProcessor;
        _filesService = filesService;
        _fileDialogService = fileDialogService;
        _previewGenerator = previewGenerator;
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
