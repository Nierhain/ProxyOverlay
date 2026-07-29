using System;
using System.Collections.Generic;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using ProxyOverlay.Models;

namespace ProxyOverlay.ViewModels;

public partial class ImageOverlayPreview : ObservableObject
{
    private bool _isUpdatingSelection;

    public ImageOverlayPreview(string filePath, IReadOnlyList<OverlayChoice> overlayChoices,
        OverlayChoice selectedOverlay)
    {
        FilePath = filePath;
        FileName = System.IO.Path.GetFileName(filePath);
        OverlayChoices = overlayChoices;
        OverlayFrames = GetFrames(overlayChoices);
        CardTypes = GetCardTypes(overlayChoices);
        _isUpdatingSelection = true;
        SelectedOverlay = selectedOverlay;
        SelectedFrame = selectedOverlay.Frame;
        SelectedCardType = selectedOverlay.CardType;
        _isUpdatingSelection = false;
    }

    public string FilePath { get; }
    public string FileName { get; }
    public IReadOnlyList<OverlayChoice> OverlayChoices { get; private set; }
    public IReadOnlyList<string> OverlayFrames { get; private set; }
    public IReadOnlyList<string> CardTypes { get; private set; }

    [ObservableProperty]
    public partial Bitmap? Preview { get; set; }

    [ObservableProperty]
    public partial OverlayChoice SelectedOverlay { get; set; }

    [ObservableProperty]
    public partial string SelectedFrame { get; set; }

    [ObservableProperty]
    public partial string SelectedCardType { get; set; }

    public event EventHandler? OverlayChanged;

    partial void OnSelectedOverlayChanged(OverlayChoice value)
    {
        if (!_isUpdatingSelection)
        {
            _isUpdatingSelection = true;
            SelectedFrame = value.Frame;
            SelectedCardType = value.CardType;
            _isUpdatingSelection = false;
        }

        OverlayChanged?.Invoke(this, EventArgs.Empty);
    }

    partial void OnSelectedFrameChanged(string value) => SelectOverlay();

    partial void OnSelectedCardTypeChanged(string value) => SelectOverlay();

    public void UpdateOverlayChoices(IReadOnlyList<OverlayChoice> choices, OverlayChoice? preferredChoice = null)
    {
        var selectedPath = SelectedOverlay?.FilePath;
        var selectedName = SelectedOverlay?.DisplayName;
        OverlayChoices = choices;
        OverlayFrames = GetFrames(choices);
        CardTypes = GetCardTypes(choices);
        OnPropertyChanged(nameof(OverlayChoices));
        OnPropertyChanged(nameof(OverlayFrames));
        OnPropertyChanged(nameof(CardTypes));

        var selected = preferredChoice
            ?? choices.FirstOrDefault(choice =>
                string.Equals(choice.FilePath, selectedPath, StringComparison.OrdinalIgnoreCase))
            ?? choices.FirstOrDefault(choice =>
                string.Equals(choice.DisplayName, selectedName, StringComparison.Ordinal))
            ?? choices[0];

        _isUpdatingSelection = true;
        SelectedOverlay = selected;
        SelectedFrame = selected.Frame;
        SelectedCardType = selected.CardType;
        _isUpdatingSelection = false;
        OverlayChanged?.Invoke(this, EventArgs.Empty);
    }

    private void SelectOverlay()
    {
        if (_isUpdatingSelection) return;

        var selected = OverlayChoices.FirstOrDefault(choice =>
            string.Equals(choice.Frame, SelectedFrame, StringComparison.Ordinal) &&
            string.Equals(choice.CardType, SelectedCardType, StringComparison.Ordinal));

        if (selected is null)
        {
            selected = OverlayChoices.FirstOrDefault(choice =>
                string.Equals(choice.Frame, SelectedFrame, StringComparison.Ordinal) &&
                choice.CardType == "Spell");
        }

        if (selected is not null && !ReferenceEquals(selected, SelectedOverlay))
        {
            SelectedOverlay = selected;
        }
    }

    private static IReadOnlyList<string> GetFrames(IReadOnlyList<OverlayChoice> choices) =>
        choices.Select(choice => choice.Frame).Distinct(StringComparer.Ordinal).ToList();

    private static IReadOnlyList<string> GetCardTypes(IReadOnlyList<OverlayChoice> choices) =>
        choices.Select(choice => choice.CardType).Distinct(StringComparer.Ordinal).ToList();
}
