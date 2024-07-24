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
    TextBox Address { get; } = new()
    {
        Text = LocalNet.ServerAddress
    };

    NumericUpDown Port { get; } = new()
    {
        Value = LocalNet.ServerPort
    };

    TextBox PlayerName { get; } = new()
    {
        Text = LocalNet.PlayerName
    };

    TextBox Password { get; } = new()
    {
        Text = LocalNet.PlayerPassword
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

    Control OperatePannel { get; set; }

    ArchiveSelector ArchiveSelector { get; } = new();

    GamePlayer GamePlayControl { get; } = new();

    //GamePlayControl GamePlay { get; } = new();

    public override Size MinimumSize { get; set; } = new(200, 200);

    public override string InitializeName { get; }

    protected override Type FormDataType => typeof(ClientData);

    public ClientForm(string initializeName)
    {
        InitializeName = initializeName;
        Text = "Client";
        //OperatePannel = GamePlayControl;
        OperatePannel = ArchiveSelector;
        Controls.AddRange([
            Address,
            Port,
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
            OperatePannel,
            //GamePlay,
            ]);
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
        ArchiveSelector.EnableListener();
        //GamePlayControl.EnableListener();
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
            GamePlayControl.EnableListener();
            ArchiveSelector.DisableListener();
            OperatePannel = GamePlayControl;
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
            GamePlayControl.DisableListener();
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
        //var fileName = Path.GetFileName(FilePath.Text);
        //var filePath = Client.GetFileRepoPath(DirName.Text, fileName);
        //// HACK: for test
        //if (!File.Exists(filePath))
        //    File.Copy(FilePath.Text, filePath);
        LocalNet.Service.UploadFileAsync(DirName.Text, FilePath.Text);
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



    private class ClientData : FormData
    {
        public string HostAddress { get; set; } = "127.0.0.1";

        public int HostPort { get; set; } = 60;

        public string UserName { get; set; } = "";

        public string Password { get; set; } = "";

        public string DirName { get; set; } = "";

        public string FilePath { get; set; } = "";
    }

    protected override void OnLoad(object? data)
    {
        if (data is not ClientData clientData)
            return;
        Address.Text = clientData.HostAddress;
        Port.Value = clientData.HostPort;
        PlayerName.Text = clientData.UserName;
        Password.Text = clientData.Password;
        DirName.Text = clientData.DirName;
        FilePath.Text = clientData.FilePath;
    }

    protected override FormData OnSave()
    {
        return new ClientData()
        {
            HostAddress = Address.Text,
            HostPort = (int)Port.Value,
            UserName = PlayerName.Text,
            Password = Password.Text,
            DirName = DirName.Text,
            FilePath = FilePath.Text,
        };
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        LocalNet.Service.Dispose();
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

    protected override void Redraw()
    {
        base.Redraw(); 
        var width = (ClientWidth - Padding * 5) / 4;
        var top = ClientTop + Padding;
        //
        Address.Left = ClientLeft + Padding;
        Address.Top = top;
        Address.Width = width;
        //
        Port.Left = Address.Right + Padding;
        Port.Top = top;
        Port.Width = width;
        //
        PlayerName.Left = Port.Right + Padding;
        PlayerName.Top = top;
        PlayerName.Width = width;
        //
        Password.Left = PlayerName.Right + Padding;
        Password.Top = top;
        Password.Width = width;
        //
        width = (ClientWidth - Padding * 5) / 5;
        top = Password.Bottom + Padding;
        var height = ClientHeight - Address.Height - SendBox.Height - FilePath.Height - Padding * 6;
        //
        OperatePannel.Bounds = new(
            ClientLeft + Padding,
            top,
            width * 4,
            height
            );
        //
        MessageBox.Left = OperatePannel.Right + Padding;
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
