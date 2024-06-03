using LocalUtilities.FileHelper;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Graph;
using LocalUtilities.TypeToolKit.Mathematic;
using WarringStates.Events;
using WarringStates.Flow.Model;
using WarringStates.Map;
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
        AddOperations();
    }

    private void OnResize(object? sender, EventArgs e)
    {
        Relocate();
        using var g = Graphics.FromImage(Image);
        g.FillRectangle(new SolidBrush(BackColor), Bounds);
        var colWidth = (Width - Padding.Width * 3) / 3;
        var height = Height - Padding.Height * 2;
        RollRect = new Rectangle(Padding.Width, Padding.Height, colWidth * 2, height);
        RollReDraw();
        var left = RollRect.Right + Padding.Width;
        height /= 2;
        OverviewRect = new(left, Padding.Height, colWidth, height);
        g.FillRectangle(new SolidBrush(FrontColor), OverviewRect);
        var overViewPadding = Padding / 4;
        OverviewRect = new(OverviewRect.Left + overViewPadding.Width, OverviewRect.Top + overViewPadding.Height, OverviewRect.Width - overViewPadding.Width * 2, OverviewRect.Height - overViewPadding.Height * 2);
        OverviewRedraw();
        var buttonWidth = colWidth - Padding.Width * 2;
        var buttonPadding = (height - ButtonHeight * 3) / 4;
        BuildButton.Rect = new(left, OverviewRect.Bottom + buttonPadding, colWidth, ButtonHeight);
        ButtonRedraw(BuildButton);
        LoadButton.Rect = new(left, BuildButton.Rect.Bottom + buttonPadding, colWidth, ButtonHeight);
        ButtonRedraw(LoadButton);
        DeleteButton.Rect = new(left + Padding.Width, LoadButton.Rect.Bottom + buttonPadding, buttonWidth, ButtonHeight);
        ButtonRedraw(DeleteButton);
        Invalidate();
    }

    private void OverviewRedraw()
    {
        if (!LocalSaves.TryGetArchive(SelectedItemIndex, out var archive))
        {
            var random = new Random();
            var pImage = new PointBitmap((Bitmap)Image);
            pImage.LockBits();
            for (var i = 0; i < OverviewRect.Width; i++)
            {
                for (var j = 0; j < OverviewRect.Height; j++)
                {
                    var color = Color.FromArgb(255, random.Next(255), random.Next(255), random.Next(255));
                    pImage.SetPixel(i + OverviewRect.Left, j + OverviewRect.Top, color);
                }
            }
            pImage.UnlockBits();
        }
        else
        {
            using var g = Graphics.FromImage(Image);
            g.FillRectangle(new SolidBrush(FrontColor), OverviewRect);
            Atlas.Relocate(archive);
            var rect = new Rectangle(OverviewRect.Left, OverviewRect.Top, OverviewRect.Width, OverviewRect.Height - Padding.Height);
            var size = Atlas.Size.ScaleSizeOnRatio(rect.Size);
            var overview = Atlas.GetOverview(size);
            overview = overview.CopyToNewSize(size, System.Drawing.Drawing2D.InterpolationMode.Low);
            rect = new(rect.Left, rect.Top, size.Width, size.Height);
            overview.TemplateDrawOntoPart((Bitmap)Image, rect, true);
            rect = new(rect.Left, rect.Bottom, rect.Width, OverviewRect.Height - rect.Height);
            var stepper = new DateStepper();
            stepper.SetStartSpan(archive.CurrentSpan);
            g.DrawString(stepper.GetDate().ToString(), new FontData(), new SolidBrush(Color.Black), rect, new() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        }
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
