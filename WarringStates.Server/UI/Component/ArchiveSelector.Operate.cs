using AltitudeMapGenerator;
using AltitudeMapGenerator.Layout;
using WarringStates.Server.Events;
using WarringStates.Server.User;

namespace WarringStates.Server.UI.Component;

partial class ArchiveSelector
{
    private void AddOperations()
    {
        BuildButton.Click += BuildButton_Click;
        LoadButton.Click += LoadButton_Click;
        DeleteButton.Click += DeleteButton_Click;
        Selector.SelectedChanged += Selector_SelectedChanged;
    }

    private async void BuildButton_Click(object? sender, EventArgs e)
    {
        var data = new AltitudeMapData(new(300, 300), new(3, 3), new(8, 8), RiverLayout.Types.Horizontal, 2.25, 60000, 0.66f);
        var task = Task.Run(() => LocalArchives.CreateArchive(data, "new world"));
        if (await task)
        {
            Selector.Redraw();
            Selector.Invalidate();
        }
    }

    private void LoadButton_Click(object? sender, EventArgs e)
    {
        LocalArchives.LoadArchive(Selector.SelectedIndex);
    }

    private void DeleteButton_Click(object? sender, EventArgs e)
    {
        LocalArchives.Delete(Selector.SelectedIndex);
        Selector.SelectedIndex = -1;
    }

    private void Selector_SelectedChanged(object? sender, EventArgs e)
    {
        if (Selector.SelectedIndex is -1)
        {
            LoadButton.CanSelect = false;
            DeleteButton.CanSelect = false;
        }
        else
        {
            LoadButton.CanSelect = true;
            DeleteButton.CanSelect = true;
        }
        if (LocalArchives.ArchiveInfoList.TryGetValue(Selector.SelectedIndex, out var info))
            Thumbnail.SetThumbnail(LocalArchives.LoadThumbnail(info), LocalArchives.LoadCurrentSpan(info));
        else
            Thumbnail.SetThumbnailVoid();
        Thumbnail.Redraw();
        Thumbnail.Invalidate();
    }
}
