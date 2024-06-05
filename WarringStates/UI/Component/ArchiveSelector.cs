using LocalUtilities.FileHelper;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Graph;
using LocalUtilities.TypeToolKit.Mathematic;
using System.Drawing;
using WarringStates.Events;
using WarringStates.Flow.Model;
using WarringStates.Map;
using WarringStates.User;

namespace WarringStates.UI.Component;

public partial class ArchiveSelector : Displayer
{
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

    //Bitmap? Thumbnail { get; set; } = null;

    Rectangle OverviewRect { get; set; } = new();

    Rectangle ThumbnailRect { get; set; } = new();

    public ArchiveSelector()
    {
        BackColor = Color.Teal;
        AddOperations();
    }

    public void EnableListener()
    {
        LocalEvents.Hub.AddListener<Rectangle>(LocalEvents.UserInterface.MainFormOnDraw, SetBounds);
        LocalEvents.Hub.AddListener<Keys>(LocalEvents.UserInterface.KeyPressed, KeyPress);
    }

    public void DisableListener()
    {
        LocalEvents.Hub.TryRemoveListener<Rectangle>(LocalEvents.UserInterface.MainFormOnDraw, SetBounds);
        LocalEvents.Hub.TryRemoveListener<Keys>(LocalEvents.UserInterface.KeyPressed, KeyPress);
    }

    private new void KeyPress(Keys key)
    {
        if (key is not Keys.Escape)
            return;
        LocalEvents.Hub.Broadcast(LocalEvents.UserInterface.MainFormToClose);
    }

    private void SetBounds(Rectangle rect)
    {
        Bounds = rect;
        Relocate();
        using var g = Graphics.FromImage(Image);
        g.FillRectangle(new SolidBrush(BackColor), Bounds);
        var colWidth = (Width - Padding.Width * 3) / 3;
        var height = Height - Padding.Height * 2;
        RollRect = new Rectangle(Padding.Width, Padding.Height, colWidth * 2, height);
        RollReDraw();
        var padding = Padding + Padding / 4;
        var left = RollRect.Right + padding.Width;
        colWidth -= Padding.Width / 2;
        height /= 2;
        OverviewRect = new(left, padding.Height, colWidth, height - Padding.Height / 2); ThumbnailRedraw();
        var buttonPadding = (height - ButtonHeight * 3) / 4;
        BuildButton.Rect = new(left, OverviewRect.Bottom + buttonPadding, colWidth, ButtonHeight);
        ButtonRedraw(BuildButton);
        LoadButton.Rect = new(left, BuildButton.Rect.Bottom + buttonPadding, colWidth, ButtonHeight);
        ButtonRedraw(LoadButton);
        var buttonWidth = colWidth - Padding.Width * 2;
        DeleteButton.Rect = new(left + Padding.Width, LoadButton.Rect.Bottom + buttonPadding, buttonWidth, ButtonHeight);
        ButtonRedraw(DeleteButton);
        Invalidate();
    }

    private void ThumbnailRedraw()
    {
        //Thumbnail?.Dispose();
        //Thumbnail = null;
        if (!LocalSaves.TryGetArchiveInfo(SelectedItemIndex, out var info) || !info.Useable())
        {
            using var g = Graphics.FromImage(Image);
            var random = new Random();
            var pImage = new PointBitmap((Bitmap)Image);
            pImage.LockBits();
            for (var i = 0; i < OverviewRect.Width; i++)
            {
                for (var j = 0; j < OverviewRect.Height; j++)
                {
                    Color color;
                    if (random.Next() < random.Next())
                        color = BackColor;
                    else
                        color = FrontColor;
                    pImage.SetPixel(i + OverviewRect.Left, j + OverviewRect.Top, color);
                }
            }
            pImage.UnlockBits();
        }
        else
        {
            var rect = OverviewRect;
            using var g = Graphics.FromImage(Image);
            g.FillRectangle(new SolidBrush(FrontColor), rect);
            ThumbnailRect = new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height - Padding.Height);
            try
            {
                var thumbnail = (Bitmap)Image.FromFile(info.GetOverviewPath());
                var size = thumbnail.Size.ScaleSizeOnRatio(ThumbnailRect.Size);
                thumbnail = thumbnail.CopyToNewSize(size, System.Drawing.Drawing2D.InterpolationMode.Low);
                ThumbnailRect = new Rectangle(ThumbnailRect.Left + (ThumbnailRect.Width - thumbnail.Width) / 2, ThumbnailRect.Top + (ThumbnailRect.Height - thumbnail.Height) / 2, thumbnail.Width, thumbnail.Height);
                thumbnail.TemplateDrawOntoPart((Bitmap)Image, ThumbnailRect, true);
                thumbnail.Dispose();
                rect = new(rect.Left, rect.Bottom - Padding.Height, rect.Width, Padding.Height);
            }
            catch { }
            var stepper = new DateStepper();
            stepper.SetStartSpan(info.CurrentSpan);
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
