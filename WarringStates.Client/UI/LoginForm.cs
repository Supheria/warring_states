using LocalUtilities.TypeGeneral;
using WarringStates.Client.Events;
using WarringStates.Client.Map;
using WarringStates.Client.Net;
using WarringStates.Data;
using WarringStates.UI;

namespace WarringStates.Client.UI;

public partial class LoginForm : Pannel
{
    public static Color FrontColor { get; set; } = Color.White;

    public static new Color BackColor { get; set; } = Color.Teal;

    public static int ButtonHeight { get; set; } = 50;

    public static Color ButtonBackColor { get; set; } = Color.LightYellow;

    public static Color ButtonFrontColor { get; set; } = Color.DarkSlateGray;

    public override Size Padding { get; set; } = new(30, 30);

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


    ImageButton LoginButton { get; } = new()
    {
        Text = Localize.Table.Login,
        FrontColor = ButtonFrontColor,
        BackColor = ButtonBackColor,
        CanSelect = true,
    };

    ImageButton JoinButton { get; } = new()
    {
        Text = Localize.Table.Join,
        FrontColor = ButtonFrontColor,
        BackColor = ButtonBackColor,
    };

    ImageButton LogoutButton { get; } = new()
    {
        Text = Localize.Table.Logout,
        FrontColor = ButtonFrontColor,
        BackColor = ButtonBackColor,
    };

    public LoginForm()
    {
        base.BackColor = BackColor;
        Controls.AddRange([
            Address,
            Port,
            PlayerName,
            Password,
            LoginButton,
            JoinButton,
            LogoutButton,
            ]);
    }

    public override void EnableListener()
    {
        base.EnableListener();
    }

    public override void DisableListener()
    {
        base.DisableListener();
    }

    private new void KeyPress(Keys key)
    {
        if (key is Keys.Escape)
            LocalEvents.TryBroadcast(LocalEvents.UserInterface.EndGame);
    }

    protected override void SetSize()
    {
        base.SetSize();
        var colWidth = ClientWidth / 8;
        var height = Address.Height;
        var width = colWidth * 3;
        var padding = (ClientHeight - height * 4) / 5;
        var top = ClientTop + padding;
        var left = ClientLeft + colWidth;
        //
        Address.Bounds = new(
            left,
            top,
            width,
            height
            );
        //
        Port.Bounds = new(
            left,
            Address.Bottom + padding,
            width,
            height
            );
        //
        PlayerName.Bounds = new(
            left,
            Port.Bottom + padding,
            width,
            height
            );
        //
        Password.Bounds = new(
            left,
            PlayerName.Bottom + padding,
            width,
            height
            );
        //
        width = colWidth * 2;
        height = ButtonHeight;
        padding = (ClientHeight - height * 3) / 4;
        left = Password.Right + colWidth;
        //
        LoginButton.Bounds = new(
            left,
            ClientTop + padding,
            width,
            height);
        //
        JoinButton.Bounds = new(
            left,
            LoginButton.Bottom + padding,
            width,
            height);
        //
        LogoutButton.Bounds = new(
            left,
            JoinButton.Bottom + padding,
            width,
            height);
    }
}
