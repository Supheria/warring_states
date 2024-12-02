using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalUtilities.GUICore;
using System.Collections.Generic;
using WarringStates.Map;
using System.Diagnostics;
using LocalUtilities.SimpleScript;
using System.IO;
using WarringStates.Server.GUI.Models;

namespace WarringStates.Server.GUI.ViewModels;

internal partial class ArchiveListViewModel : ViewModelBase
{

    [ObservableProperty]
    List<ArchiveInfo> _archiveList = [];

    [ObservableProperty]
    ArchiveInfo? _selectedArchive = null;

    //[RelayCommand]
    //private void Load()
    //{
    //    Debug.WriteLine("OnLoad is run!");
    //}

    [RelayCommand]
    private void RefreshItems()
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
    }
}
