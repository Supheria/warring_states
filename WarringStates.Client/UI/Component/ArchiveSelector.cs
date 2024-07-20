using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Graph;
using WarringStates.Client.Events;
using WarringStates.Client.User;

namespace WarringStates.Client.UI.Component;

public partial class ArchiveSelector : Displayer
{
    private class Button(string name)
    {
        public string Name { get; } = name;

        public Rectangle Rect { get; set; } = new();

        public bool Selected { get; set; } = false;
    }

    int ButtonHeight { get; set; } = 50;

    FontData ButtonFontData { get; set; } = new()
    {
        Size = 25f,
        Style = FontStyle.Bold,

    };

    Color ButtonBackColor { get; set; } = Color.LightYellow;

    Color ButtonFrontColor { get; set; } = Color.DarkSlateGray;

    Button RefreshButton { get; } = new("刷新");

    Button JoinButton { get; } = new("加入");

    Button LogoutButton { get; } = new("登出");

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
        SizeChanged += (_, _) => SetBounds(Bounds);
    }

    public void EnableListener()
    {
        LocalEvents.TryAddListener(LocalEvents.UserInterface.ArchiveListRefreshed, RelocateArchiveList);
        LocalEvents.TryAddListener<Rectangle>(LocalEvents.UserInterface.GamePlayControlOnDraw, SetBounds);
        //LocalEvents.TryAddListener<Keys>(LocalEvents.UserInterface.KeyPressed, KeyPress);
    }

    private void RelocateArchiveList()
    {
        if (LocalArchives.Count is 0)
            SelectedItemIndex = -1;
        RollReDraw();
    }

    public void DisableListener()
    {
        LocalEvents.TryRemoveListener(LocalEvents.UserInterface.ArchiveListRefreshed, RelocateArchiveList);
        LocalEvents.TryRemoveListener<Rectangle>(LocalEvents.UserInterface.GamePlayControlOnDraw, SetBounds);
        //LocalEvents.TryRemoveListener<Keys>(LocalEvents.UserInterface.KeyPressed, KeyPress);
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
        RefreshButton.Rect = new(left, OverviewRect.Bottom + buttonPadding, colWidth, ButtonHeight);
        ButtonRedraw(RefreshButton);
        JoinButton.Rect = new(left, RefreshButton.Rect.Bottom + buttonPadding, colWidth, ButtonHeight);
        ButtonRedraw(JoinButton);
        var buttonWidth = colWidth - Padding.Width * 2;
        LogoutButton.Rect = new(left + Padding.Width, JoinButton.Rect.Bottom + buttonPadding, buttonWidth, ButtonHeight);
        ButtonRedraw(LogoutButton);
        Invalidate();
    }

    private void ThumbnailRedraw()
    {
        //Thumbnail?.Dispose();
        //Thumbnail = null;
        if (!LocalArchives.TryGetArchiveId(SelectedItemIndex, out var info))
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
        // TODO: draw thumbnail for player
        else
        {
            //var rect = OverviewRect;
            //using var g = Graphics.FromImage(Image);
            //g.FillRectangle(new SolidBrush(FrontColor), rect);
            //ThumbnailRect = new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height - Padding.Height);
            //try
            //{
            //    var thumbnail = (Bitmap)Image.FromFile(info.GetThumbnailPath());
            //    var size = thumbnail.Size.ScaleSizeOnRatio(ThumbnailRect.Size);
            //    thumbnail = thumbnail.CopyToNewSize(size, System.Drawing.Drawing2D.InterpolationMode.Low);
            //    ThumbnailRect = new Rectangle(ThumbnailRect.Left + (ThumbnailRect.Width - thumbnail.Width) / 2, ThumbnailRect.Top + (ThumbnailRect.Height - thumbnail.Height) / 2, thumbnail.Width, thumbnail.Height);
            //    thumbnail.TemplateDrawOntoPart((Bitmap)Image, ThumbnailRect, true);
            //    thumbnail.Dispose();
            //    rect = new(rect.Left, rect.Bottom - Padding.Height, rect.Width, Padding.Height);
            //}
            //catch { }
            //var stepper = new DateStepper();
            //stepper.SetStartSpan(info.CurrentSpan);
            //g.DrawString(stepper.GetDate().ToString(), new FontData(), new SolidBrush(Color.Black), rect, new() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
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
