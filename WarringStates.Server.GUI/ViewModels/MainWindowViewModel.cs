using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using LocalUtilities.GUICore;
using System.Collections.Generic;
using System.ComponentModel;
using WarringStates.Server.GUI.Models;
using WarringStates.User;

namespace WarringStates.Server.GUI.ViewModels;

internal partial class MainWindowViewModel : ViewModelBase
{
    public ThumbnailViewModel ThumbnailViewModel { get; } = new ThumbnailViewModel()
    {
        BackColor = Colors.LightGray
    };

    public ArchiveListViewModel ArchiveListViewModel { get; } = new();

    [ObservableProperty]
    List<Player> _players = [new("aa", ""), new("bb", "")];

    [ObservableProperty]
    Player? _selectedPlayer = null;

    public MainWindowViewModel()
    {
        ArchiveListViewModel.PropertyChanged += ArchiveListViewModel_PropertyChanged;
    }

    private void ArchiveListViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(ArchiveListViewModel.SelectedArchive):
                AtlasEx.SetCurrentArchive(ArchiveListViewModel.SelectedArchive);
                ThumbnailViewModel.Source?.Dispose();
                ThumbnailViewModel.Source = AtlasEx.GetThumbnail();
                break;
        }
    }
}
