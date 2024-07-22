using LocalUtilities.FileHelper;
using LocalUtilities.SimpleScript;
using LocalUtilities.TypeGeneral;
using WarringStates.Client.Events;
using WarringStates.Client.Net;
using WarringStates.Client.User;
using WarringStates.User;

namespace WarringStates.Client.UI;

public partial class ClientForm : ResizeableForm
{
    ClientService Client { get; } = new();

    TextBox HostAddress { get; } = new()
    {
        Text = "127.0.0.1"
    };

    NumericUpDown HostPort { get; } = new()
    {
        Value = 60,
    };

    TextBox PlayerName { get; } = new()
    {
        Text = "admin"
    };

    TextBox Password { get; } = new()
    {
        Text = "password"
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

    ComboBox DirName { get; } = new();

    TextBox FilePath { get; } = new();

    Button FilePathButton { get; } = new()
    {
        Text = "..."
    };

    Button UploadButton { get; } = new()
    {
        Text = "Upload"
    };

    Button DownloadButton { get; } = new()
    {
        Text = "Download"
    };

    ListBox UserList { get; } = new()
    {
        SelectionMode = SelectionMode.MultiExtended
    };

    GamePlayControl GamePlay { get; } = new();

    public override Size MinimumSize { get; set; } = new(200, 200);

    public override string InitializeName { get; }

    protected override Type DataType => typeof(ClientData);

    public ClientForm(string initializeName)
    {
        InitializeName = initializeName;
        Text = "Client";
        Controls.AddRange([
            HostAddress,
            HostPort,
            PlayerName,
            Password,
            MessageBox,
            UserList,
            SendBox,
            SendButton,
            DirName,
            FilePath,
            FilePathButton,
            UploadButton,
            DownloadButton,
            GamePlay,
            ]);
        OnLoadForm += ClientForm_OnLoadForm;
        OnSaveForm += ClientForm_OnSaveForm;
        FormClosing += (_, _) => Client.Dispose();
        OnDrawClient += ClientForm_OnDrawClient;
        SendButton.Click += SendButton_Click;
        FilePathButton.Click += FilePathButton_Click;
        UploadButton.Click += UploadButton_Click;
        DownloadButton.Click += (_, _) => Client.DownLoadFileAsync(DirName.Text, Path.GetFileName(FilePath.Text));
        Client.OnLog += UpdateMessage;
        Client.OnLogined += Client_OnConnected;
        Client.OnClosed += Client_OnDisconnected;
        Client.OnProcessing += UpdateFormText;
        Client.OnUpdatePlayerList += Client_OnUpdateUserList;
        EnableListener();
    }

    private void UploadButton_Click(object? sender, EventArgs e)
    {
        var fileName = Path.GetFileName(FilePath.Text);
        var filePath = Client.GetFileRepoPath(DirName.Text, fileName);
        // HACK: for test
        if (!File.Exists(filePath))
            File.Copy(FilePath.Text, filePath);
        Client.UploadFileAsync(DirName.Text, fileName);
    }

    private void SendButton_Click(object? sender, EventArgs e)
    {
        //Client.SendMessage(SendBox.Text, "");
        var index = UserList.SelectedIndex;
        if (index is -1)
        {
            UpdateMessage("no selected user to send message");
            return;
        }
        foreach (var item in UserList.SelectedItems)
            Client.SendMessage(SendBox.Text, (string)item);
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

    private void Client_OnDisconnected()
    {
        BeginInvoke(() =>
        {
            HostAddress.Enabled = true;
            HostPort.Enabled = true;
            PlayerName.Enabled = true;
            Password.Enabled = true;
            Update();
        });
        //GamePlay.FinishGame();
    }

    private void Client_OnConnected()
    {
        BeginInvoke(() =>
        {
            HostAddress.Enabled = false;
            HostPort.Enabled = false;
            PlayerName.Enabled = false;
            Password.Enabled = false;
            Update();
        });
        // TODO: for test
        //ArchiveManager.LoadArchive(0, out var archive);
        //Atlas.Relocate(archive);
        //SourceLand.TryBuild(new(22, 22), Atlas.LandMap, SourceLand.Types.TerraceLand, out var s1);
        //archive.SourceLands.Add(s1);
        //Atlas.Relocate(archive);

        //GamePlay.StartGame();
    }

    private class ClientData : FormData
    {
        public string HostAddress { get; set; } = "127.0.0.1";

        public int HostPort { get; set; } = 60;

        public string UserName { get; set; } = "";

        public string Password { get; set; } = "";

        public string DirName { get; set; } = "";

        public string FilePath { get; set; } = "";
    }

    private void ClientForm_OnLoadForm(object? data)
    {
        if (data is not ClientData clientData)
            return;
        HostAddress.Text = clientData.HostAddress;
        HostPort.Value = clientData.HostPort;
        PlayerName.Text = clientData.UserName;
        Password.Text = clientData.Password;
        DirName.Text = clientData.DirName;
        FilePath.Text = clientData.FilePath;
    }

    private FormData ClientForm_OnSaveForm()
    {
        return new ClientData()
        {
            HostAddress = HostAddress.Text,
            HostPort = (int)HostPort.Value,
            UserName = PlayerName.Text,
            Password = Password.Text,
            DirName = DirName.Text,
            FilePath = FilePath.Text,
        };
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

    private void ClientForm_OnDrawClient()
    {
        var width = (ClientWidth - Padding * 5) / 4;
        var top = ClientTop + Padding;
        //
        HostAddress.Left = ClientLeft + Padding;
        HostAddress.Top = top;
        HostAddress.Width = width;
        //
        HostPort.Left = HostAddress.Right + Padding;
        HostPort.Top = top;
        HostPort.Width = width;
        //
        PlayerName.Left = HostPort.Right + Padding;
        PlayerName.Top = top;
        PlayerName.Width = width;
        //
        Password.Left = PlayerName.Right + Padding;
        Password.Top = top;
        Password.Width = width;
        //
        width = (ClientWidth - Padding * 5) / 5;
        top = Password.Bottom + Padding;
        var height = ClientHeight - HostAddress.Height - SendBox.Height - FilePath.Height - Padding * 6;
        //
        GamePlay.Left = ClientLeft + Padding;
        GamePlay.Top = top;
        GamePlay.Width = width * 3;
        GamePlay.Height = height;
        //
        MessageBox.Left = GamePlay.Right + Padding;
        //MessageBox.Left = ClientLeft + Padding;
        MessageBox.Top = top;
        MessageBox.Width = width;
        //MessageBox.Width = width * 4;
        MessageBox.Height = height;
        //
        UserList.Left = MessageBox.Right + Padding;
        UserList.Top = top;
        UserList.Width = width;
        UserList.Height = height;
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
        //
        width = (ClientWidth - Padding * 10) / 12;
        var width2x = width * 3;
        top = SendButton.Bottom + Padding;
        //
        DirName.Left = ClientLeft + Padding;
        DirName.Top = top;
        DirName.Width = width2x;
        //
        FilePath.Left = DirName.Right + Padding;
        FilePath.Top = top;
        FilePath.Width = width2x + Padding;
        //
        FilePathButton.Left = FilePath.Right + Padding;
        FilePathButton.Top = top;
        FilePathButton.Width = width;
        //
        UploadButton.Left = FilePathButton.Right + Padding;
        UploadButton.Top = top;
        UploadButton.Width = width2x;
        //
        DownloadButton.Left = UploadButton.Right + Padding;
        DownloadButton.Top = top;
        DownloadButton.Width = width2x;
    }
}
