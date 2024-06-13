using LocalUtilities.Net;
using LocalUtilities.TypeGeneral;

namespace WarringStates.UI;

internal class ServerForm : ResizeableForm
{
    public override string LocalName => nameof(ServerForm);

    TcpServer Server { get; } = new(10, 1024);

    Button SwitchButton { get; } = new()
    {
        Text = "Start"
    };

    NumericUpDown Port { get; } = new()
    {
        Value = 60,
    };

    RichTextBox MessageBox { get; } = new();

    protected override void InitializeComponent()
    {
        Text = "tcp server";
        Controls.AddRange([
            SwitchButton,
            Port,
            MessageBox,
            ]);
        OnDrawingClient += DrawClient;
        SwitchButton.Click += SwitchButton_Click;
        Server.ClientNumberChange += Server_ClientNumberChange;
    }

    private void Server_ClientNumberChange(bool connect, AsyncClientProfile client)
    {
        if (connect)
        {
            UpdateMessage($"{client.RemoteEndPoint} connect");
        }
        else
        {
            UpdateMessage($"{client.RemoteEndPoint} disconnect");
        }
    }

    private void UpdateMessage(string message)
    {
        lock (MessageBox)
        {
            Invoke(new Action(() =>
            {
                MessageBox.Text += $"{message}\n";
                Update();
            }));
        }
    }

    private void SwitchButton_Click(object? sender, EventArgs e)
    {
        if (Server.IsStart)
        {
            Server.Stop();
            if (!Server.IsStart)
                SwitchButton.Text = "Start";
            else
                System.Windows.Forms.MessageBox.Show($"close server failed");
        }
        else
        {
            Server.Start((int)Port.Value, out var message);
            if (Server.IsStart)
                SwitchButton.Text = "Close";
            else
                System.Windows.Forms.MessageBox.Show($"start server failed");
        }
    }

    private void DrawClient()
    {
        var width = ClientWidth / 5;
        //
        // PortSelector
        //
        Port.Left = ClientLeft + width;
        Port.Top = ClientTop + Padding;
        Port.Width = width;
        //
        // SwitchButton
        //
        SwitchButton.Left = Port.Right + width;
        SwitchButton.Top = ClientTop + Padding;
        SwitchButton.Width = width;
        //
        // RichTextBox
        //
        MessageBox.Left = ClientLeft + Padding;
        MessageBox.Top = SwitchButton.Bottom + Padding;
        MessageBox.Width = ClientWidth - Padding * 2;
        MessageBox.Height = ClientHeight - Port.Height - Padding * 3;
    }
}
