using CommunityToolkit.Mvvm.ComponentModel;
using LocalUtilities.GUICore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Map;

namespace WarringStates.Server.GUI.ViewModels;

internal partial class ArchiveListViewModel : ViewModelBase
{

    [ObservableProperty]
    List<ArchiveInfo> _archiveList = [];

    [ObservableProperty]
    ArchiveInfo? _selectedArchive = null;
}
