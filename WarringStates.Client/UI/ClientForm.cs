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

    Pannel OperatePannel { get; set; }

    ArchiveSelector ArchiveSelector { get; } = new();

    GamePlayer GamePlayer { get; } = new();

    public override Size MinimumSize { get; set; } = new(200, 200);

    public override string InitializeName { get; }

    public ClientForm(string initializeName)
    {
        InitializeName = initializeName;
        Text = "Client";
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
            ]);
        ArchiveSelector.EnableListener();
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
