using LocalUtilities.FileHelper;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Graph;
using WarringStates.Events;
using WarringStates.User;

namespace WarringStates.UI;

public partial class InitializeDisplayer : Displayer
{
    Rectangle OverviewRect { get; set; } = new();

    private class Button(string name)
    {
        public string Name { get; } = name;

        public Rectangle Rect { get; set; } = new();

        public bool Selected { get; set; } = false;
    }

    int ButtonHeight { get; set; } = 50;

    FontData ButtonFontData { get; set; } = new(nameof(ButtonFontData))
    {
        Size = 25f,
        Style = FontStyle.Bold,

    };

    Color ButtonBackColor { get; set; } = Color.LightYellow;

    Color ButtonFrontColor { get; set; } = Color.DarkSlateGray;

    Button BuildButton { get; } = new("新建");

    Button LoadButton { get; } = new("加载");

    Button DeleteButton { get; } = new("删除");

    new Size Padding { get; } = new(30, 30);

    Color FrontColor { get; set; } = Color.White;

    Color SelectedColor { get; set; } = Color.Gold;

    public InitializeDisplayer()
    {
        BackColor = Color.Teal;
        SizeChanged += OnResize;
    }

    private void OnResize(object? sender, EventArgs e)
    {
        Relocate();
        using var g = Graphics.FromImage(Image);
        g.FillRectangle(new SolidBrush(BackColor), Bounds);
        var colWidth = (Width - Padding.Width * 3) / 3;
        var height = Height - Padding.Height * 2;
        RollRect = new Rectangle(Padding.Width, Padding.Height, colWidth * 2, height);
        RollReSize();
        var left = RollRect.Right + Padding.Width;
        height /= 2;
        OverviewRect = new(left, Padding.Height, colWidth, height);
        var buttonWidth = colWidth - Padding.Width * 2;
        var buttonPadding = (height - ButtonHeight * 3) / 4;
        BuildButton.Rect = new(left, OverviewRect.Bottom + buttonPadding, colWidth, ButtonHeight);
        ButtonRedraw(BuildButton);
        LoadButton.Rect = new(left, BuildButton.Rect.Bottom + buttonPadding, colWidth, ButtonHeight);
        ButtonRedraw(LoadButton);
        DeleteButton.Rect = new(left + Padding.Width, LoadButton.Rect.Bottom + buttonPadding, buttonWidth - Padding.Width, ButtonHeight);
        ButtonRedraw(DeleteButton);
        g.FillRectangle(new SolidBrush(FrontColor), new(OverviewRect.Left, OverviewRect.Top, OverviewRect.Width, OverviewRect.Height));
        Invalidate();
    }

    private void ButtonRedraw(Button button)
    {
        ButtonRedraw(button, button.Selected);
    }

    private void ButtonRedraw(Button button, bool selected)
    {
        using var g = Graphics.FromImage(Image);
        if (selected)
        {
            g.FillRectangle(new SolidBrush(ButtonFrontColor), button.Rect);
            g.DrawUniformString(button.Rect, button.Name, ButtonHeight * 0.618f, ButtonBackColor, ButtonFontData);
        }
        else
        {
            g.FillRectangle(new SolidBrush(ButtonBackColor), button.Rect);
            g.DrawUniformString(button.Rect, button.Name, ButtonHeight * 0.618f, ButtonFrontColor, ButtonFontData);
        }
        Invalidate();
    }
}
