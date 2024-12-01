using Avalonia.Controls;
using Avalonia.Interactivity;
using LocalUtilities.SimpleScript;
using System;
using System.Collections.Generic;
using System.IO;
using WarringStates.Map;
using WarringStates.Server.GUI.Models;

namespace WarringStates.Server.GUI.Views;

public partial class ArchiveList : ListBox
{
    protected override Type StyleKeyOverride => typeof(ListBox);

    public ArchiveList()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        RefreshItems();
    }

    public void RefreshItems()
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
        ItemsSource = archives;
    }
}
