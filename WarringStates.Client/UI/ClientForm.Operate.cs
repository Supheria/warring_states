using WarringStates.Client.Events;
using WarringStates.Client.Net;

namespace WarringStates.Client.UI;

partial class ClientForm
{
    protected override void AddOperation()
    {
        base.AddOperation();
        KeyPreview = true;
        SendButton.Click += SendButton_Click;
        FilePathButton.Click += FilePathButton_Click;
        UploadButton.Click += UploadButton_Click;
        DownloadButton.Click += (_, _) => LocalNet.Service.DownLoadFileAsync(DirName.Text, Path.GetFileName(FilePath.Text));
        LocalNet.Service.OnLog += UpdateMessage;
        LocalNet.Service.OnLogined += Service_OnLogined; ;
        LocalNet.Service.OnClosed += Service_OnClosed; ;
        LocalNet.Service.OnProcessing += UpdateFormText;
        LocalNet.Service.OnUpdatePlayerList += Client_OnUpdateUserList;
        LocalEvents.TryAddListener(LocalEvents.UserInterface.StartGamePlay, StartGame);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        LocalEvents.TryBroadcast(LocalEvents.UserInterface.KeyPressed, e);
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        LocalNet.Service.Dispose();
    }

    private void Service_OnLogined()
    {
        //StartGame();
    }

    private void Service_OnClosed()
    {
        EndGame();
    }

    private void StartGame()
    {
        BeginInvoke(() =>
        {
            Controls.Remove(OperatePannel);
            GamePlayer.EnableListener();
            Login.DisableListener();
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
            Login.EnableListener();
            GamePlayer.DisableListener();
            OperatePannel = Login;
            Controls.Add(OperatePannel);
            Redraw();
        });
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
        {
            var name = (string)item;
            LocalNet.Service.SendMessage(SendBox.Text, name);
        }
    }

    private void Client_OnUpdateUserList(string[] playerList)
    {
        BeginInvoke(() =>
        {
            UserList.Items.Clear();
            foreach (var player in playerList)
            {
                if (player == LocalNet.PlayerName)
                    continue;
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
