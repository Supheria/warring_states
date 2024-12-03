using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    int _Port = 60;

    [ObservableProperty]
    string _SwitchButtonContent = "开启";

    [ObservableProperty]
    List<Player> _Players = [new("aa", ""), new("bb", "")];

    [ObservableProperty]
    Player? _SelectedPlayer = null;

    public MainWindowViewModel()
    {
        ArchiveListViewModel.PropertyChanged += ArchiveListViewModel_PropertyChanged;
        LocalNet.Server.OnStart += Server_OnStart;
        LocalNet.Server.OnClose += Server_OnClose;
    }

    private void Server_OnStart()
    {
        ArchiveListViewModel.IsEnabled = false;
        SwitchButtonContent = "关闭";

    }

    private void Server_OnClose()
    {
        ArchiveListViewModel.IsEnabled = true;
        SwitchButtonContent = "开启";
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

    [RelayCommand]
    private void SwitchServer()
    {
        if (ArchiveListViewModel.SelectedArchive is null)
        {
            LocalNet.Server.Close();
            return;
        }
        if (LocalNet.Server.IsStart)
            LocalNet.Server.Close();
        else
            LocalNet.Server.Start(Port);
    }

    //private void CreateArchive
}
