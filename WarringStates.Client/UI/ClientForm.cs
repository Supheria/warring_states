using LocalUtilities.TypeGeneral;
using WarringStates.Client.Net;

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
        MinimumSize = new(800, 550);
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
        var height = Address.Height;
        //
        Address.Bounds = new(
            ClientLeft + Padding,
            top,
            width,
            height
            );
        //
        Port.Bounds = new(
            Address.Right + Padding,
            top,
            width,
            height
            );
        //
        PlayerName.Bounds = new(
            Port.Right + Padding,
            top,
            width,
            height
            );
        //
        Password.Bounds = new(
            PlayerName.Right + Padding,
            top,
            width,
            height
            );
        //
        width = (ClientWidth - Padding * 5) / 6;
        top = Password.Bottom + Padding;
        height = ClientHeight - Address.Height - SendBox.Height - FilePath.Height - Padding * 6;
        //
        OperatePannel.Bounds = new(
            ClientLeft + Padding,
            top,
            width * 4,
            height
            );
        //
        MessageBox.Bounds = new(
            OperatePannel.Right + Padding,
            top,
            width,
            height
            );
        //
        UserList.Bounds = new(
             MessageBox.Right + Padding,
             top,
             width,
             height
            );
        //
        width = (ClientWidth - Padding * 3) / 4;
        height = SendBox.Height;
        top = MessageBox.Bottom + Padding;
        //
        SendBox.Bounds = new(
            ClientLeft + Padding,
            top,
            width * 3,
            height
            );
        //
        SendButton.Bounds = new(
            SendBox.Right + Padding,
            top,
            width,
            height
            );
        //
        width = (ClientWidth - Padding * 10) / 12;
        var width2x = width * 3;
        top = SendButton.Bottom + Padding;
        //
        DirName.Bounds = new(
            ClientLeft + Padding,
            top,
            width2x,
            height
            );
        //
        FilePath.Bounds = new(
            DirName.Right + Padding,
            top,
            width2x + Padding,
            height
            );
        //
        FilePathButton.Bounds = new(
            FilePath.Right + Padding,
            top,
            width,
            height
            );
        //
        UploadButton.Bounds = new(
            FilePathButton.Right + Padding,
            top,
            width2x,
            height
            );
        //
        DownloadButton.Bounds = new(
            UploadButton.Right + Padding,
            top,
            width2x,
            height
            );
    }
}
