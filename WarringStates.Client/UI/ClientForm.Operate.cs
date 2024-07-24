using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Client.Net;

namespace WarringStates.Client.UI;

partial class ClientForm
{
    protected override void AddOperation()
    {
        base.AddOperation();
        SendButton.Click += SendButton_Click;
        FilePathButton.Click += FilePathButton_Click;
        UploadButton.Click += UploadButton_Click;
        DownloadButton.Click += (_, _) => LocalNet.Service.DownLoadFileAsync(DirName.Text, Path.GetFileName(FilePath.Text));
        Address.TextChanged += Address_TextChanged;
        Port.ValueChanged += Port_ValueChanged;
        PlayerName.TextChanged += PlayerName_TextChanged;
        Password.TextChanged += Password_TextChanged;
        LocalNet.Service.OnLog += UpdateMessage;
        LocalNet.Service.OnLogined += Service_OnLogined; ;
        LocalNet.Service.OnClosed += Service_OnClosed; ;
        LocalNet.Service.OnProcessing += UpdateFormText;
        LocalNet.Service.OnUpdatePlayerList += Client_OnUpdateUserList;
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        LocalNet.Service.Dispose();
    }

    private void Service_OnLogined()
    {
        BeginInvoke(() =>
        {
            Address.Enabled = false;
            Port.Enabled = false;
            PlayerName.Enabled = false;
            Password.Enabled = false;
            Update();
        });
        StartGame();
    }

    private void Service_OnClosed()
    {
        BeginInvoke(() =>
        {
            Address.Enabled = true;
            Port.Enabled = true;
            PlayerName.Enabled = true;
            Password.Enabled = true;
            Update();
        });
        EndGame();
    }

    private void StartGame()
    {
        BeginInvoke(() =>
        {
            Controls.Remove(OperatePannel);
            GamePlayer.EnableListener();
            ArchiveSelector.DisableListener();
            OperatePannel = GamePlayer;
            Controls.Add(OperatePannel);
            Redraw();
        });
    }

    private void EndGame()
    {
        BeginInvoke(() =>
        {
            Controls.Remove(OperatePannel);
            ArchiveSelector.EnableListener();
            GamePlayer.DisableListener();
            OperatePannel = ArchiveSelector;
            Controls.Add(OperatePannel);
            Redraw();
        });
    }

    private void Password_TextChanged(object? sender, EventArgs e)
    {
        LocalNet.PlayerPassword = Password.Text;
    }

    private void PlayerName_TextChanged(object? sender, EventArgs e)
    {
        LocalNet.PlayerName = PlayerName.Text;
    }

    private void Port_ValueChanged(object? sender, EventArgs e)
    {
        LocalNet.ServerPort = (int)Port.Value;
    }

    private void Address_TextChanged(object? sender, EventArgs e)
    {
        LocalNet.ServerAddress = Address.Text;
    }

    private void UploadButton_Click(object? sender, EventArgs e)
    {
        LocalNet.Service.UploadFileAsync(DirName.Text, FilePath.Text);
    }

    private void SendButton_Click(object? sender, EventArgs e)
    {
        var index = UserList.SelectedIndex;
        if (index is -1)
        {
            UpdateMessage("no selected user to send message");
            return;
        }
        foreach (var item in UserList.SelectedItems)
            LocalNet.Service.SendMessage(SendBox.Text, (string)item);
    }

    private void Client_OnUpdateUserList(string[] playersName)
    {
        BeginInvoke(() =>
        {
            UserList.Items.Clear();
            foreach (var player in playersName)
            {
                if (player != PlayerName.Text)
                    UserList.Items.Add(player);
            }
            Update();
        });
    }

    private void FilePathButton_Click(object? sender, EventArgs e)
    {
        var file = new OpenFileDialog();
        if (file.ShowDialog() is DialogResult.Cancel)
            return;
        FilePath.Text = file.FileName;
    }

    private void UpdateMessage(string message)
    {
        BeginInvoke(() =>
        {
            MessageBox.Text += $"{message}\n";
            Update();
        });
    }

    private void UpdateFormText(string text)
    {
        BeginInvoke(() =>
        {
            Text = $"client - {text}";
            Update();
        });
    }

}
