using LocalUtilities.TypeGeneral;
using WarringStates.Client.Events;
using WarringStates.Client.Map;
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
        var colWidth = ClientWidth - Padding.Width * 2;
        var height = ClientHeight - Padding.Height * 2;
        //
        var padding = Padding + Padding / 4;
        var left = ClientLeft + padding.Width;
        colWidth -= Padding.Width / 2;
        height /= 2;
        var buttonPadding = (height - ButtonHeight * 3) / 4;
        //
        LoginButton.Bounds = new(
            left,
            ClientHeight + buttonPadding,
            colWidth,
            ButtonHeight);
        //
        JoinButton.Bounds = new(
            left,
            LoginButton.Bottom + buttonPadding,
            colWidth,
            ButtonHeight);
        //
        var buttonWidth = colWidth - Padding.Width * 2;
        LogoutButton.Bounds = new(
            left + Padding.Width,
            JoinButton.Bottom + buttonPadding,
            buttonWidth,
            ButtonHeight);
    }
}
