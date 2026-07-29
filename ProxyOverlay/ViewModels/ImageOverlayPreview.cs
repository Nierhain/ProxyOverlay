using System;
using System.Collections.Generic;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ProxyOverlay.ViewModels;

public sealed record OverlayChoice(string DisplayName, string FilePath);

public partial class ImageOverlayPreview : ObservableObject
{
    public ImageOverlayPreview(string filePath, IReadOnlyList<OverlayChoice> overlayChoices,
        OverlayChoice selectedOverlay)
    {
        FilePath = filePath;
        FileName = System.IO.Path.GetFileName(filePath);
        OverlayChoices = overlayChoices;
        SelectedOverlay = selectedOverlay;
    }

    public string FilePath { get; }
    public string FileName { get; }
    public IReadOnlyList<OverlayChoice> OverlayChoices { get; private set; }

    [ObservableProperty]
    public partial Bitmap? Preview { get; set; }

    [ObservableProperty]
    public partial OverlayChoice SelectedOverlay { get; set; }

    public event EventHandler? OverlayChanged;

    partial void OnSelectedOverlayChanged(OverlayChoice value)
    {
        OverlayChanged?.Invoke(this, EventArgs.Empty);
    }

    public void UpdateOverlayChoices(IReadOnlyList<OverlayChoice> choices)
    {
        var selectedPath = SelectedOverlay.FilePath;
        var selectedName = SelectedOverlay.DisplayName;
        OverlayChoices = choices;
        OnPropertyChanged(nameof(OverlayChoices));
        SelectedOverlay = choices.FirstOrDefault(choice =>
            string.Equals(choice.FilePath, selectedPath, StringComparison.OrdinalIgnoreCase))
            ?? choices.FirstOrDefault(choice =>
                string.Equals(choice.DisplayName, selectedName, StringComparison.Ordinal))
            ?? choices[0];
    }
}
