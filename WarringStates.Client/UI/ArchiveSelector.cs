using LocalUtilities.TypeGeneral;
using WarringStates.Client.Events;
using WarringStates.Client.Map;
using WarringStates.Client.User;
using WarringStates.UI;

namespace WarringStates.Client.UI;

public partial class ArchiveSelector : Pannel
{
    public static Color FrontColor { get; set; } = Color.White;

    public static new Color BackColor { get; set; } = Color.Teal;

    public static int ButtonHeight { get; set; } = 50;

    public static Color ButtonBackColor { get; set; } = Color.LightYellow;

    public static Color ButtonFrontColor { get; set; } = Color.DarkSlateGray;

    public override Size Padding { get; set; } = new(30, 30);

    Selector Selector { get; } = new()
    {
        FrontColor = FrontColor,
        BackColor = BackColor,
    };

    Thumbnail Thumbnail { get; } = new()
    {
        FrontColor = FrontColor,
        BackColor = BackColor,
    };

    ImageButton LoginButton { get; } = new()
    {
        Text = "登录",
        FrontColor = ButtonFrontColor,
        BackColor = ButtonBackColor,
        CanSelect = true,
    };

    ImageButton JoinButton { get; } = new()
    {
        Text = "加入",
        FrontColor = ButtonFrontColor,
        BackColor = ButtonBackColor,
    };

    ImageButton LogoutButton { get; } = new()
    {
        Text = "登出",
        FrontColor = ButtonFrontColor,
        BackColor = ButtonBackColor,
    };

    public ArchiveSelector()
    {
        base.BackColor = BackColor;
        Controls.AddRange([
            Selector,
            Thumbnail,
            LoginButton,
            JoinButton,
            LogoutButton,
            ]);
    }

    public override void EnableListener()
    {
        base.EnableListener();
        LocalEvents.TryAddListener(LocalEvents.UserInterface.ArchiveListRefreshed, RefreshSelector);
        LocalEvents.TryAddListener(LocalEvents.UserInterface.CurrentArchiveChange, SetPlayerArchive);
    }

    public override void DisableListener()
    {
        base.DisableListener();
        LocalEvents.TryRemoveListener(LocalEvents.UserInterface.ArchiveListRefreshed, RefreshSelector);
        LocalEvents.TryRemoveListener(LocalEvents.UserInterface.CurrentArchiveChange, SetPlayerArchive);
    }

    private void SetPlayerArchive()
    {
        if (LocalArchives.CurrentArchive is null)
            Thumbnail.SetThumbnailVoid();
        else
        {
            var thumbnail = Atlas.GetOverview(Thumbnail.ClientSize);
            Thumbnail.SetThumbnail(thumbnail, LocalArchives.CurrentArchive.CurrentSpan);
        }
        Thumbnail.Redraw();
        Thumbnail.Invalidate();
    }

    private void RefreshSelector()
    {
        Selector.ArchiveInfoList = LocalArchives.ArchiveInfoList;
        Selector.Redraw();
        Selector.Invalidate();
    }

    private new void KeyPress(Keys key)
    {
        if (key is Keys.Escape)
            LocalEvents.TryBroadcast(LocalEvents.UserInterface.EndGame);
    }

    protected override void SetSize()
    {
        base.SetSize();
        var colWidth = (ClientWidth - Padding.Width * 3) / 3;
        var height = ClientHeight - Padding.Height * 2;
        //
        Selector.Bounds = new(
            ClientLeft + Padding.Width,
            ClientTop + Padding.Height,
            colWidth * 2,
            height);
        //
        var padding = Padding + Padding / 4;
        var left = Selector.Right + padding.Width;
        colWidth -= Padding.Width / 2;
        height /= 2;
        //
        Thumbnail.Bounds = new(
            left,
            padding.Height,
            colWidth,
            height - Padding.Height / 2);
        //
        var buttonPadding = (height - ButtonHeight * 3) / 4;
        //
        LoginButton.Bounds = new(
            left,
            Thumbnail.Bottom + buttonPadding,
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
