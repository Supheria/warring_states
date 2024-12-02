using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalUtilities.GUICore;
using System.Collections.Generic;
using WarringStates.Map;
using System.Diagnostics;
using LocalUtilities.SimpleScript;
using System.IO;
using WarringStates.Server.GUI.Models;
using System.Threading.Tasks;
using System.ComponentModel;

namespace WarringStates.Server.GUI.ViewModels;

internal partial class ArchiveListViewModel : ViewModelBase
{

    [ObservableProperty]
    List<ArchiveInfo> _ArchiveList = [];

    [ObservableProperty]
    ArchiveInfo? _SelectedArchive = null;

    [ObservableProperty]
    bool _IsEnabled = true;

    [RelayCommand]
    private async Task RefreshItems()
    {
        IsEnabled = false;
        await Task.Run(() =>
        {
            var archives = new List<ArchiveInfo>();
            foreach (var folder in new DirectoryInfo(AtlasEx.RootPath).GetDirectories())
            {
                try
                {
                    var archiveId = folder.Name;
                    var archiveInfo = SerializeTool.DeserializeFile<ArchiveInfo>(AtlasEx.GetArchiveInfoPath(archiveId));
                    if (archiveInfo is not null && archiveInfo.Id == archiveId)
                        archives.Add(archiveInfo);
                }
                catch { }
            }
            ArchiveList = archives;
        });
        IsEnabled = true;
    }
}
