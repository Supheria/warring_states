using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Graph;
using System.Drawing;
using WarringStates.Client.Events;
using WarringStates.Client.User;
using WarringStates.Map.Terrain;
using WarringStates.UI;

namespace WarringStates.Client.UI;

public partial class ArchiveSelector : Pannel
{
    public static new Size Padding { get; } = new(30, 30);

    public static Color FrontColor { get; set; } = Color.White;

    public static new Color BackColor { get; set; } = Color.Teal;

    public static int ButtonHeight { get; set; } = 50;

    public static Color ButtonBackColor { get; set; } = Color.LightYellow;

    public static Color ButtonFrontColor { get; set; } = Color.DarkSlateGray;

    Selector Selector { get; } = new()
    {
        FrontColor = FrontColor,
        BackColor = BackColor,
    };

    Thumbnail Thumbnail { get; } = new()
    {
        FrontColor = FrontColor,
        BackColor = BackColor,
        Padding = Padding,
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
        AddOperations();
        Controls.AddRange([
            Selector,
            Thumbnail,
            LoginButton,
            JoinButton,
            LogoutButton,
            ]);
    }

    public void EnableListener()
    {
        LocalEvents.TryAddListener(LocalEvents.UserInterface.ArchiveListRefreshed, RefreshSelector);
        //LocalEvents.TryAddListener<Rectangle>(LocalEvents.UserInterface.GamePlayControlOnDraw, SetBounds);
        LocalEvents.TryAddListener<ThumbnailRedrawArgs>(LocalEvents.UserInterface.ThumbnailFetched, SetThumbnail);
        //LocalEvents.TryAddListener<Keys>(LocalEvents.UserInterface.KeyPressed, KeyPress);
    }

    private void SetThumbnail(ThumbnailRedrawArgs info)
    {
        var thumbnail = new Bitmap(info.Width, info.Height);
        var g = Graphics.FromImage(thumbnail);
        g.Clear(Color.White);
        var pThumbnail = new PointBitmap(thumbnail);
        pThumbnail.LockBits();
        foreach (var land in info.OwnerShip)
        {
            foreach (var point in land.GetPoints())
            {
                pThumbnail.SetPixel(point.X, point.Y, land.Color);
            }
        }
        pThumbnail.UnlockBits();
        Thumbnail.SetThumbnail(thumbnail, info.CurrentSpan);
        Thumbnail.Redraw();
        Thumbnail.Invalidate();
    }

    public void DisableListener()
    {
        LocalEvents.TryRemoveListener(LocalEvents.UserInterface.ArchiveListRefreshed, RefreshSelector);
        //LocalEvents.TryRemoveListener<Rectangle>(LocalEvents.UserInterface.GamePlayControlOnDraw, SetBounds);
        //LocalEvents.TryRemoveListener<Keys>(LocalEvents.UserInterface.KeyPressed, KeyPress);
    }

    private void RefreshSelector()
    {
        Selector.ArchiveInfoList = LocalArchives.ArchiveInfoList;
        Selector.Redraw();
        Selector.Invalidate();
    }

    private new void KeyPress(Keys key)
    {
        if (key is not Keys.Escape)
            return;
        LocalEvents.TryBroadcast(LocalEvents.UserInterface.MainFormToClose);
    }

    private void SetBounds(Rectangle rect)
    {
        Bounds = rect;
    }

    protected override void SetSize()
    {
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
