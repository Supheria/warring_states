using LocalUtilities.TypeGeneral;
using WarringStates.Client.Net;

namespace WarringStates.Client.UI;

public partial class ClientForm : ResizeableForm
{
    
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

    LoginForm Login { get; } = new();

    GamePlayer GamePlayer { get; } = new();

    public override Size MinimumSize { get; set; } = new(200, 200);

    public override string InitializeName { get; }

    public ClientForm(string initializeName)
    {
        InitializeName = initializeName;
        Text = "Client";
        MinimumSize = new(800, 550);
        OperatePannel = Login;
        Controls.AddRange([
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
        Login.EnableListener();
    }

    protected override void Redraw()
    {
        base.Redraw();
        var top = ClientTop + Padding;
        var width = (ClientWidth - Padding * 4) / 10;
        var height = ClientHeight - SendBox.Height - FilePath.Height - Padding * 6;
        //
        OperatePannel.Bounds = new(
            ClientLeft + Padding,
            top,
            width * 6,
            height
            );
        //
        MessageBox.Bounds = new(
            OperatePannel.Right + Padding,
            top,
            width * 3,
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
