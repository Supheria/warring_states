using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalUtilities.SimpleScript;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WarringStates.Map;
using WarringStates.Server.GUI.Models;

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
