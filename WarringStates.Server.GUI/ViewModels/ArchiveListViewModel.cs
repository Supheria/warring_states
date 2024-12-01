using CommunityToolkit.Mvvm.ComponentModel;
using LocalUtilities.GUICore;
using System.Collections.Generic;
using WarringStates.Map;

namespace WarringStates.Server.GUI.ViewModels;

internal partial class ArchiveListViewModel : ViewModelBase
{

    [ObservableProperty]
    List<ArchiveInfo> _archiveList = [];

    [ObservableProperty]
    ArchiveInfo? _selectedArchive = null;
}
