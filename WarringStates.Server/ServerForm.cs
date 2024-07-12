using LocalUtilities.TypeGeneral;
using WarringStates.Flow;
using WarringStates.Server.Component;
using WarringStates.Server.User;

namespace WarringStates.Server;

internal class ServerForm : ResizeableForm
{
    public override string LocalName => nameof(ServerForm);

    Net.Server Server { get; set; } = new();

    NumericUpDown Port { get; } = new()
    {
        Value = 60,
    };

    Button SwitchButton { get; } = new()
    {
        Text = "Start"
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
            SwitchButton,
            ParallelCount,
            MessageBox,
            SendBox,
            SendButton,
            //ArchiveSelector
            ]);
        OnDrawClient += DrawClient;
        SwitchButton.Click += SwitchButton_Click;
        SendButton.Click += SendButton_Click;
        Server.OnLog += UpdateMessage;
        Server.OnConnectionCountChange += Host_OnParallelRemainChange;
        //Host.Start((int)Port.Value);
        LocalEvents.Hub.TryAddListener<Archive>(LocalEvents.UserInterface.ArchiveSelected, LoadArchive);
        ArchiveSelector.EnableListener();
        LocalArchives.ReLocate();
    }

    private void LoadArchive(Archive archive)
    {
        //Atlas.Relocate(archive);
        SpanFlow.Relocate(archive.Info.CurrentSpan);
        LocalEvents.Hub.TryBroadcast(LocalEvents.UserInterface.StartGamePlay);
        LocalEvents.Hub.TryBroadcast(LocalEvents.Flow.SwichFlowState);
        //ArchiveSelector.DisableListener();
        DrawClient();
    }

    private void SendButton_Click(object? sender, EventArgs e)
    {
        Server.BroadcastMessage(SendBox.Text);
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

    private void SwitchButton_Click(object? sender, EventArgs e)
    {
        if (Server.IsStart)
        {
            Server.Close();
            if (!Server.IsStart)
                SwitchButton.Text = "Start";
            else
                System.Windows.Forms.MessageBox.Show($"close server failed");
        }
        else
        {
            Server.Start((int)Port.Value);
            if (Server.IsStart)
                SwitchButton.Text = "Close";
            else
                System.Windows.Forms.MessageBox.Show($"start server failed");
        }
    }

    private void DrawClient()
    {
        var width = (ClientWidth - Padding * 5) / 3;
        var top = ClientTop + Padding;
        //
        Port.Left = ClientLeft + Padding;
        Port.Top = top;
        Port.Width = width;
        //
        SwitchButton.Left = Port.Right + Padding;
        SwitchButton.Top = top;
        SwitchButton.Width = width;
        //
        ParallelCount.Left = SwitchButton.Right + Padding;
        ParallelCount.Top = top;
        ParallelCount.Width = width;
        //
        width = (ClientWidth - Padding * 3) / 5;
        top = SwitchButton.Bottom + Padding;
        var height = ClientHeight - Port.Height - SendButton.Height - Padding * 4;
        //
        ArchiveSelector.Left = ClientLeft + Padding;
        ArchiveSelector.Top = top;
        ArchiveSelector.Width = width * 4;
        ArchiveSelector.Height = height;
        //
        //MessageBox.Left = ArchiveSelector.Right + Padding;
        MessageBox.Left = ClientLeft + Padding;
        MessageBox.Top = top;
        MessageBox.Width = ClientWidth - Padding * 2;
        MessageBox.Height = height;
        //
        top = MessageBox.Bottom + Padding;
        //
        SendBox.Left = ClientLeft + Padding;
        SendBox.Top = top;
        SendBox.Width = width * 2 + Padding;
        //
        SendButton.Left = SendBox.Right + Padding;
        SendButton.Top = top;
        SendButton.Width = width;
    }
}
