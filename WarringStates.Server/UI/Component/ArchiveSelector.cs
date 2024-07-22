using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Graph;
using LocalUtilities.TypeToolKit.Mathematic;
using WarringStates.Flow.Model;
using WarringStates.Server.Events;
using WarringStates.Server.User;
using WarringStates.UI;

namespace WarringStates.Server.UI.Component;

public partial class ArchiveSelector : Control
{
    public static new Size Padding { get; set; } = new(30, 30);

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
        Padding = Padding,
    };

    ImageButton BuildButton { get; } = new()
    {
        Text = "新建",
        FrontColor = ButtonFrontColor,
        BackColor = ButtonBackColor,
        CanSelect = true,
    };

    ImageButton LoadButton { get; } = new()
    {
        Text = "加载",
        FrontColor = ButtonFrontColor,
        BackColor = ButtonBackColor,
    };

    ImageButton DeleteButton { get; } = new()
    {
        Text = "删除",
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
            BuildButton,
            LoadButton, 
            DeleteButton,
            ]);
    }

    public void EnableListener()
    {
        //LocalEvents.TryAddListener<Rectangle>(LocalEvents.UserInterface.MainFormOnDraw, SetBounds);
        //LocalEvents.TryAddListener<Keys>(LocalEvents.UserInterface.KeyPressed, KeyPress);
        LocalEvents.TryAddListener(LocalEvents.UserInterface.ArchiveListRefreshed, RefreshSelector);
    }

    private void RefreshSelector()
    {
        Selector.ArchiveInfoList = LocalArchives.ArchiveInfoList;
        Selector.Redraw();
        Selector.Invalidate();
    }

    public void DisableListener()
    {
        //LocalEvents.TryRemoveListener<Rectangle>(LocalEvents.UserInterface.MainFormOnDraw, SetBounds);
        //LocalEvents.TryRemoveListener<Keys>(LocalEvents.UserInterface.KeyPressed, KeyPress);
    }

    private new void KeyPress(Keys key)
    {
        if (key is not Keys.Escape)
            return;
        LocalEvents.TryBroadcast(LocalEvents.UserInterface.MainFormToClose);
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        BeginInvoke(SetSize);
    }

    private void SetSize()
    {
        var colWidth = (Width - Padding.Width * 3) / 3;
        var height = Height - Padding.Height * 2;
        //
        Selector.Bounds = new(Padding.Width, Padding.Height, colWidth * 2, height);
        //
        var padding = Padding + Padding / 4;
        var left = Selector.Right + padding.Width;
        colWidth -= Padding.Width / 2;
        height /= 2;
        //
        Thumbnail.Bounds = new(left, padding.Height, colWidth, height - Padding.Height / 2);
        //
        var buttonPadding = (height - ButtonHeight * 3) / 4;
        //
        BuildButton.Bounds = new(left, Thumbnail.Bottom + buttonPadding, colWidth, ButtonHeight);
        //
        LoadButton.Bounds = new(left, BuildButton.Bottom + buttonPadding, colWidth, ButtonHeight);
        //
        var buttonWidth = colWidth - Padding.Width * 2;
        //
        DeleteButton.Bounds = new(left + Padding.Width, LoadButton.Bottom + buttonPadding, buttonWidth, ButtonHeight);
    }
}
