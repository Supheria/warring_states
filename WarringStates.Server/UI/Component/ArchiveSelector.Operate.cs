using AltitudeMapGenerator;
using AltitudeMapGenerator.Layout;
using WarringStates.Server.Net;
using WarringStates.Server.User;

namespace WarringStates.Server.UI.Component;

partial class ArchiveSelector
{
    protected override void AddOperation()
    {
        base.AddOperation();
        BuildButton.Click += BuildButton_Click;
        SwitchButton.Click += SwitchButton_Click;
        DeleteButton.Click += DeleteButton_Click;
        Selector.SelectedChanged += Selector_SelectedChanged;
        LocalNet.Server.OnStart += Server_OnStart; ;
        LocalNet.Server.OnClose += Server_OnClose; ;
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
        if (Progressor.Progressing)
        {
            MessageBox.Show("world is in building");
            return;
        }
        //var data = new AltitudeMapData(new(300, 300), new(3, 3), new(8, 8), RiverLayout.Types.Horizontal, 2.25, 60000, 0.66f);
        var data = new AltitudeMapData(new(1000, 1000), new(6, 8), new(8, 8), RiverLayout.Types.ForwardSlash, 7, 650000, 0.75f);
        Progressor.Progressing = true;
        SetSize();
        Invalidate(true);
        var task = Task.Run(() => LocalArchives.CreateArchive(data, "new world", Progressor));
        if (await task)
        {
            Selector.Redraw();
            Progressor.Progressing = false;
            SetSize();
            Invalidate(true);
        }
    }

    private void SwitchButton_Click(object? sender, EventArgs e)
    {
        if (LocalNet.Server.IsStart)
            LocalNet.Close();
        else
            LocalNet.Start();
    }

    private void DeleteButton_Click(object? sender, EventArgs e)
    {
        LocalArchives.Delete(Selector.SelectedIndex);
        Selector.SelectedIndex = -1;
    }

    private void Selector_SelectedChanged(object? sender, EventArgs e)
    {
        if (Selector.SelectedIndex is -1)
            DeleteButton.CanSelect = false;
        else
            DeleteButton.CanSelect = true;
        if (LocalArchives.ArchiveInfoList.TryGetValue(Selector.SelectedIndex, out var info) &&
            LocalArchives.LoadThumbnail(info, out var thumbnail))
            Thumbnail.SetThumbnail(thumbnail, LocalArchives.LoadCurrentSpan(info));
        else
            Thumbnail.SetThumbnailVoid();
        Thumbnail.Redraw();
        Thumbnail.Invalidate();
    }
}
