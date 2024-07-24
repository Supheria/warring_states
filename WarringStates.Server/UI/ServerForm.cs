using LocalUtilities.TypeGeneral;
using WarringStates.Flow;
using WarringStates.Server.Events;
using WarringStates.Server.UI.Component;
using WarringStates.Server.User;
using WarringStates.Server.Net;

namespace WarringStates.Server.UI;

internal class ServerForm : ResizeableForm
{
    public override string InitializeName => nameof(ServerForm);

    NumericUpDown Port { get; } = new()
    {
        Value = LocalNet.Port,
    };

    Label ParallelCount { get; } = new()
    {
        TextAlign = ContentAlignment.MiddleCenter,
        Text = "0",
    };

    RichTextBox MessageBox { get; } = new();

    TextBox SendBox { get; } = new()
    {
        Multiline = true,
    };

    Button SendButton { get; } = new()
    {
        Text = "Send",
    };

    ArchiveSelector ArchiveSelector { get; } = new();

    SpanFlow SpanFlow { get; set; } = new();

    AnimateFlow AnimateFlow { get; set; } = new();

    public ServerForm()
    {
        Text = "Server";
        Controls.AddRange([
            Port,
            ParallelCount,
            MessageBox,
            SendBox,
            SendButton,
            ArchiveSelector
            ]);
        SendButton.Click += SendButton_Click;
        Port.ValueChanged += (_, _) => LocalNet.Port = (int)Port.Value;
        LocalNet.Server.OnLog += UpdateMessage;
        LocalNet.Server.OnConnectionCountChange += Host_OnParallelRemainChange;
        LocalNet.Server.OnStart += () => Port.Enabled = false;
        LocalNet.Server.OnClose += () => Port.Enabled = true;
        //Host.Start((int)Port.Value);
        LocalEvents.TryAddListener(LocalEvents.UserInterface.ArchiveToLoad, LoadArchive);
        ArchiveSelector.EnableListener();
        LocalArchives.ReLocate();
    }

    private void LoadArchive()
    {
        if (LocalArchives.CurrentArchive is null)
            return;
        SpanFlow.Relocate(LocalArchives.CurrentArchive.CurrentSpan);
        LocalEvents.TryBroadcast(LocalEvents.UserInterface.StartGamePlay);
        LocalEvents.TryBroadcast(LocalEvents.Flow.SwichFlowState);
        //ArchiveSelector.DisableListener();
        //DrawClient();
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

    protected override void Redraw()
    {
        base.Redraw(); 
        var width = (ClientWidth - Padding * 5) / 2;
        var top = ClientTop + Padding;
        var height = Port.Height;
        //
        Port.Bounds = new(
            ClientLeft + Padding,
            top,
            width,
            height
            );
        ParallelCount.Bounds = new(
            Port.Right + Padding,
            top,
            width,
            height
            );
        //
        width = (ClientWidth - Padding * 3) / 5;
        top = Port.Bottom + Padding;
        height = ClientHeight - Port.Height - SendButton.Height - Padding * 4;
        //
        ArchiveSelector.Bounds = new(
            ClientLeft + Padding,
            top,
            width * 4,
            height
            );
        //
        MessageBox.Bounds = new(
            ArchiveSelector.Right + Padding,
            top,
            width,
            height
            );
        //
        top = MessageBox.Bottom + Padding;
        height = SendBox.Height;
        //
        SendBox.Bounds = new(
            ClientLeft + Padding,
            top,
            width * 2 + Padding,
            height
            );
        //
        SendButton.Bounds = new(
            SendBox.Right + Padding,
            top,
            width,
            height
            );
    }
}
