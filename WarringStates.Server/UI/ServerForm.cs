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

    ServiceManager Server { get; set; } = new();

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
            ArchiveSelector
            ]);
        SwitchButton.Click += SwitchButton_Click;
        SendButton.Click += SendButton_Click;
        Server.OnLog += UpdateMessage;
        Server.OnConnectionCountChange += Host_OnParallelRemainChange;
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

    protected override void Redraw()
    {
        base.Redraw(); 
        var width = (ClientWidth - Padding * 5) / 3;
        var top = ClientTop + Padding;
        var height = Port.Height;
        //
        Port.Bounds = new(
            ClientLeft + Padding,
            top,
            width,
            height
            );
        //
        SwitchButton.Bounds = new(
            Port.Right + Padding,
            top,
            width,
            height
            );
        //
        ParallelCount.Bounds = new(
            SwitchButton.Right + Padding,
            top,
            width,
            height
            );
        //
        width = (ClientWidth - Padding * 3) / 5;
        top = SwitchButton.Bottom + Padding;
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
