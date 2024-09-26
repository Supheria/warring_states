using AltitudeMapGenerator;
using AltitudeMapGenerator.Layout;
using WarringStates.Data;
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
        Thumbnail.SetThumbnail(Atlas.LoadThumbnail(), Atlas.LoadCurrentSpan());
        Thumbnail.Redraw();
        Thumbnail.Invalidate();
    }


    protected override void AddOperation()
    {
        base.AddOperation();
        BuildButton.Click += BuildButton_Click;
        SwitchButton.Click += SwitchButton_Click;
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
    }

    private void Server_OnClose()
    {
        SwitchButton.Text = "开启";
        SwitchButton.Redraw();
        SwitchButton.Invalidate();
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
        await Task.Run(() => Atlas.CreateArchive(data, "new world", Progressor));
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
        Atlas.Delete(Selector.SelectedIndex);
        Selector.SelectedIndex = -1;

    }

    private void Selector_SelectedChanged(object? sender, EventArgs e)
    {
        if (Selector.SelectedIndex is -1)
        {
            DeleteButton.CanSelect = false;
            Thumbnail.SetThumbnailVoid();
            Thumbnail.Redraw();
            Thumbnail.Invalidate();
        }
        else
            DeleteButton.CanSelect = true;
        Atlas.SetCurrentArchive(Selector.SelectedIndex);
    }
}
