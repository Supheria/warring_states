using LocalUtilities.TypeGeneral;
using WarringStates.Server.Events;
using WarringStates.Server.User;
using WarringStates.UI;

namespace WarringStates.Server.UI.Component;

public partial class ArchiveSelector : Pannel
{
    public override Size Padding { get; set; } = new(30, 30);

    public static Color FrontColor { get; set; } = Color.White;

    public static new Color BackColor { get; set; } = Color.Teal;

    public static Color ButtonBackColor { get; set; } = Color.LightYellow;

    public static Color ButtonFrontColor { get; set; } = Color.DarkSlateGray;

    public static int ButtonHeight { get; set; } = 50;

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

    ImageButton SwitchButton { get; } = new()
    {
        Text = "开启",
        FrontColor = ButtonFrontColor,
        BackColor = ButtonBackColor,
        CanSelect = true,
    };

    ImageButton BuildButton { get; } = new()
    {
        Text = "新建",
        FrontColor = ButtonFrontColor,
        BackColor = ButtonBackColor,
        CanSelect = true,
    };

    ImageButton DeleteButton { get; } = new()
    {
        Text = "删除",
        FrontColor = ButtonFrontColor,
        BackColor = ButtonBackColor,
    };

    Progressor Progressor { get; } = new();

    public int ProgressorHeight { get; set; } = 30;

    public ArchiveSelector()
    {
        base.BackColor = BackColor;
        Controls.AddRange([
            Selector,
            Thumbnail,
            BuildButton,
            SwitchButton,
            DeleteButton,
            Progressor,
            ]);
    }

    public override void EnableListener()
    {
        base.EnableListener();
        LocalEvents.TryAddListener(LocalEvents.UserInterface.ArchiveListRefreshed, RefreshSelector);
    }

    public override void DisableListener()
    {
        base.DisableListener();
        LocalEvents.TryRemoveListener(LocalEvents.UserInterface.ArchiveListRefreshed, RefreshSelector);
    }

    private void RefreshSelector()
    {
        Selector.ArchiveInfoList = LocalArchive.Archives;
        Selector.Redraw();
        Selector.Invalidate();
    }


    private new void KeyPress(Keys key)
    {
        if (key is Keys.Escape)
            LocalEvents.TryBroadcast(LocalEvents.UserInterface.MainFormToClose);
    }

    protected override void SetSize()
    {
        base.SetSize();
        var colWidth = (ClientWidth - Padding.Width * 3) / 3;
        var height = ClientHeight - Padding.Height * 2;
        if (Progressor.Progressing)
            height -= ProgressorHeight + Padding.Height;
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
        SwitchButton.Bounds = new(
            left,
            Thumbnail.Bottom + buttonPadding,
            colWidth,
            ButtonHeight);
        //
        BuildButton.Bounds = new(
            left,
            SwitchButton.Bottom + buttonPadding,
            colWidth,
            ButtonHeight);
        //
        var buttonWidth = colWidth - Padding.Width * 2;
        //
        DeleteButton.Bounds = new(
            left + Padding.Width,
            BuildButton.Bottom + buttonPadding,
            buttonWidth,
            ButtonHeight);
        //
        if (Progressor.Progressing)
            Progressor.Bounds = new(
                Padding.Width,
                Selector.Bottom + Padding.Height,
                Width - Padding.Width * 2,
                ProgressorHeight);
    }
}
