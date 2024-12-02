using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using WarringStates.Server.GUI.Models;
using WarringStates.User;

namespace WarringStates.Server.GUI.ViewModels;

internal partial class MainWindowViewModel : ViewModelBase
{
    public ThumbnailViewModel ThumbnailViewModel { get; } = new();

    public ArchiveListViewModel ArchiveListViewModel { get; } = new();

    [ObservableProperty]
    List<Player> _Players = [new("aa", ""), new("bb", "")];

    [ObservableProperty]
    Player? _SelectedPlayer = null;

    public MainWindowViewModel()
    {
        ArchiveListViewModel.PropertyChanged += ArchiveListViewModel_PropertyChanged;
    }

    private async void ArchiveListViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(ArchiveListViewModel.SelectedArchive):
                ArchiveListViewModel.IsEnabled = false;
                AtlasEx.SetCurrentArchive(ArchiveListViewModel.SelectedArchive);
                ThumbnailViewModel.SetSourceLoading();
                await Task.Run(() => ThumbnailViewModel.Source = AtlasEx.GetThumbnail());
                ArchiveListViewModel.IsEnabled = true;
                break;
        }
    }
}
