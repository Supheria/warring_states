using LocalUtilities.IocpNet.Serve;
using LocalUtilities.TypeGeneral;

namespace WarringStates.UI;

internal class ServerForm : ResizeableForm
{
    public override string LocalName => nameof(ServerForm);

    IocpHost Host { get; } = new(1000);

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
        OnDrawClient += DrawClient;
        SwitchButton.Click += SwitchButton_Click;
        //Host.ClientNumberChange += Server_ClientNumberChange;
        //Host.ReceiveClientData += Server_ReceiveClientData;
    }

    //private void Server_ReceiveClientData(AsyncClientProfile client, byte[] buff)
    //{
    //    UpdateMessage($"{client.RemoteEndPoint}: {Encoding.UTF8.GetString(buff)}");
    //}

    //private void Server_ClientNumberChange(bool add, AsyncClientProfile client)
    //{
    //    if (add)
    //    {
    //        UpdateMessage($"{client.RemoteEndPoint} connect");
    //    }
    //    else
    //    {
    //        UpdateMessage($"{client.RemoteEndPoint} disconnect");
    //    }
    //}

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
        if (Host.IsStart)
        {
            Host.Stop();
            if (!Host.IsStart)
                SwitchButton.Text = "Start";
            else
                System.Windows.Forms.MessageBox.Show($"close server failed");
        }
        else
        {
            Host.Start((int)Port.Value);
            if (Host.IsStart)
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
