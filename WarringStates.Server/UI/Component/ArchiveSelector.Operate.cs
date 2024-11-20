using AltitudeMapGenerator;
using AltitudeMapGenerator.Layout;
using WarringStates.Server.Events;
using WarringStates.Server.Map;
using WarringStates.Server.Net;

namespace WarringStates.Server.UI.Component;

partial class ArchiveSelector
{
    public override void EnableListener()
    {
        base.EnableListener();
        LocalEvents.TryAddListener(LocalEvents.UserInterface.ArchiveListRefreshed, RefreshSelector);
        LocalEvents.TryAddListener(LocalEvents.UserInterface.CurrentArchiveChange, ResetThumbnail);
    }

    public override void DisableListener()
    {
        base.DisableListener();
        LocalEvents.TryRemoveListener(LocalEvents.UserInterface.ArchiveListRefreshed, RefreshSelector);
        LocalEvents.TryRemoveListener(LocalEvents.UserInterface.CurrentArchiveChange, ResetThumbnail);
    }

    private void ResetThumbnail()
    {
        Thumbnail.SetThumbnail(AtlasEx.GetThumbnail(), AtlasEx.CurrentSpan);
        Thumbnail.Redraw();
        Thumbnail.Invalidate();
    }


    protected override void AddOperation()
    {
        base.AddOperation();
        SwitchButton.Click += SwitchButton_Click;
        BuildButton.Click += BuildButton_Click;
        DeleteButton.Click += DeleteButton_Click;
        Selector.IndexChanged += Selector_SelectedChanged;
        LocalNet.Server.OnStart += Server_OnStart;
        LocalNet.Server.OnClose += Server_OnClose;
    }

    private void Server_OnStart()
    {
        SwitchButton.Text = "关闭";
        SwitchButton.Redraw();
        SwitchButton.Invalidate();
        BuildButton.CanSelect = false;
        DeleteButton.CanSelect = false;
    }

    private void Server_OnClose()
    {
        SwitchButton.Text = "开启";
        SwitchButton.Redraw();
        SwitchButton.Invalidate();
        BuildButton.CanSelect = true;
        DeleteButton.CanSelect = true;
    }

    private async void BuildButton_Click(object? sender, EventArgs e)
    {
        if (Progressor.Visible)
        {
            MessageBox.Show("world is in building");
            return;
        }
        var data = new AltitudeMapData(new(300, 300), new(3, 3), new(8, 8), RiverLayout.Types.Horizontal, 2.25, 60000, 0.66f);
        //var data = new AltitudeMapData(new(1000, 1000), new(6, 8), new(8, 8), RiverLayout.Types.ForwardSlash, 7, 650000, 0.75f);
        Progressor.Visible = true;
        SetSize();
        Invalidate(true);
        await Task.Run(() => AtlasEx.CreateArchive(data, "new world", Progressor));
        Selector.Redraw();
        Progressor.Visible = false;
        SetSize();
        Invalidate(true);
    }

    private void SwitchButton_Click(object? sender, EventArgs e)
    {
        LocalNet.Switch();
    }

    private void DeleteButton_Click(object? sender, EventArgs e)
    {
        AtlasEx.Delete(Selector.SelectedIndex);
        Selector.SelectedIndex = -1;
    }

    private void Selector_SelectedChanged(object? sender, EventArgs e)
    {
        if (Selector.SelectedIndex is -1)
        {
            SwitchButton.CanSelect = false;
            DeleteButton.CanSelect = false;
            Thumbnail.SetThumbnailVoid();
            Thumbnail.Redraw();
            Thumbnail.Invalidate();
        }
        else
        {
            SwitchButton.CanSelect = true;
            DeleteButton.CanSelect = true;
        }
        AtlasEx.SetCurrentArchive(Selector.SelectedIndex);
    }
}
