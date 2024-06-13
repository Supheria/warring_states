using LocalUtilities.Net;
using LocalUtilities.TypeGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WarringStates.UI;

public class ClientForm : ResizeableForm
{
    public override string LocalName => nameof(ClientForm);

    TcpClient Client { get; } = new();

    TextBox ServerAddress { get; } = new()
    {
        Text = "127.0.0.1"
    };

    NumericUpDown ServerPort { get; } = new()
    {
        Value = 60,
    };

    Button SwitchButton { get; } = new()
    {
        Text = "Connect",
    };

    RichTextBox MessageBox { get; } = new();

    TextBox SendBox { get; } = new();

    Button SendButton { get; } = new()
    {
        Text = "Send",
    };

    protected override void InitializeComponent()
    {
        Text = "tcp client";
        Controls.AddRange([
            ServerAddress,
            ServerPort,
            SwitchButton,
            MessageBox,
            SendBox,
            SendButton,
            ]);
        OnDrawingClient += DrawClient;
        SwitchButton.Click += SwitchButton_Click;
        Client.ServerStopEvent += Client_ServerStopEvent;
    }

    private void Client_ServerStopEvent()
    {
        if (!Client.IsConnect)
            UpdateSwitchButtonText(true);
    }

    private void SwitchButton_Click(object? sender, EventArgs e)
    {
        if (Client.IsConnect)
        {
            Client.Disconnect(out var message);
            if (!Client.IsConnect)
                UpdateSwitchButtonText(true);
            else
                System.Windows.Forms.MessageBox.Show($"disconnect to server failed: {message}");
        }
        else
        {
            Client.Connect(ServerAddress.Text, (int)ServerPort.Value, out var message);
            if (Client.IsConnect)
                UpdateSwitchButtonText(false);
            else
                System.Windows.Forms.MessageBox.Show($"connect to server failed: {message}");
        }
    }

    private void UpdateSwitchButtonText(bool connect)
    {
        lock (SwitchButton)
        {
            Invoke(new Action(() =>
            {
                SwitchButton.Text = connect ? "Connect" : "Disconnect";
                Update();
            }));
        }
    }

    private void DrawClient()
    {
        var width = ClientWidth / 7;
        //
        ServerAddress.Left = ClientLeft + width;
        ServerAddress.Top = ClientTop + Padding;
        ServerAddress.Width = width;
        //
        ServerPort.Left = ServerAddress.Right + width;
        ServerPort.Top = ClientTop + Padding;
        ServerPort.Width = width;
        //
        SwitchButton.Left = ServerPort.Right + width;
        SwitchButton.Top = ClientTop + Padding;
        SwitchButton.Width = width;
        //
        MessageBox.Left = ClientLeft + Padding;
        MessageBox.Top = SwitchButton.Bottom + Padding;
        MessageBox.Width = ClientWidth - Padding * 2;
        MessageBox.Height = ClientHeight - ServerAddress.Height - SendBox.Height - Padding * 4;
        //
        width = (ClientWidth - Padding * 3) / 4;
        //
        SendBox.Left = ClientLeft + Padding;
        SendBox.Top = MessageBox.Bottom + Padding;
        SendBox.Width = width * 3;
        //
        SendButton.Left = SendBox.Right + Padding;
        SendButton.Top = MessageBox.Bottom + Padding;
        SendButton.Width = width;
    }
}
