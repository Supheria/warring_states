using WarringStates.Server.Map;
using WarringStates.Server.Net;

namespace WarringStates.Server.UI;

partial class ServerForm
{
    protected override void AddOperation()
    {
        base.AddOperation();
        SendButton.Click += SendButton_Click;
        Port.ValueChanged += (_, _) => LocalNet.Port = (int)Port.Value;
        LocalNet.Server.OnLog += UpdateMessage;
        LocalNet.Server.OnConnectionCountChange += Host_OnParallelRemainChange;
        LocalNet.Server.OnStart += () => Port.Enabled = false;
        LocalNet.Server.OnClose += () => Port.Enabled = true;
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        AtlasEx.RefreshArchiveList();
    }

    private void SendButton_Click(object? sender, EventArgs e)
    {
        LocalNet.Server.BroadcastMessage(SendBox.Text);
    }

    private void Host_OnParallelRemainChange(int args)
    {
        BeginInvoke(() =>
        {
            ParallelCount.Text = args.ToString();
            Update();
        });
    }

    private void UpdateMessage(string message)
    {
        BeginInvoke(new Action(() =>
        {
            MessageBox.Text += $"{message}\n";
            Update();
        }));
    }
}
