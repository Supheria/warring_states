using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using WarringStates.Map;

namespace WarringStates.Server.GUI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    List<ArchiveInfo> _archiveList = [new("存档一", 0, 0), new("存档二", 0, 0)];

    [ObservableProperty]
    ArchiveInfo? _selectedArchive = null;

    [ObservableProperty]
    Color _thumbnailBackColor = Colors.LightGray;
}
