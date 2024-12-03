using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace WarringStates.Server.GUI.Views;

internal partial class Thumbnail : Control
{
    public Color BackColor
    {
        get => GetValue(BackColorProperty);
        set => SetValue(BackColorProperty, value);
    }
    public static readonly StyledProperty<Color> BackColorProperty =
        AvaloniaProperty.Register<Thumbnail, Color>(nameof(BackColor));

    public Color FrontColor
    {
        get => GetValue(FrontColorProperty);
        set => SetValue(FrontColorProperty, value);
    }
    public static readonly StyledProperty<Color> FrontColorProperty =
        AvaloniaProperty.Register<Thumbnail, Color>(nameof(FrontColor));

    public double BorderThickness
    {
        get => GetValue(BorderThicknessProperty);
        set => SetValue(BorderThicknessProperty, value);
    }
    public static readonly StyledProperty<double> BorderThicknessProperty =
        AvaloniaProperty.Register<Thumbnail, double>(nameof(FrontColor));

    public Bitmap? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }
    public static readonly StyledProperty<Bitmap?> SourceProperty =
        AvaloniaProperty.Register<Thumbnail, Bitmap?>(nameof(Source));

    public Thumbnail()
    {
        InitializeComponent();
        AffectsRender<Thumbnail>(SourceProperty);
    }

    public sealed override void Render(DrawingContext context)
    {
        base.Render(context);
        var rect = new Rect(Bounds.Size);
        if (rect.Width <= 0 || rect.Width <= 0)
            return;
        var border = BorderThickness;
        var border2x = BorderThickness * 2;
        context.FillRectangle(new SolidColorBrush(BackColor), rect);
        rect = new(border, border, rect.Width - border2x, rect.Height - border2x);
        context.FillRectangle(new SolidColorBrush(FrontColor), rect);
        var source = Source;
        if (source is null)
            return;
        // TODO: will remove this judge after WriteableBitmap.CreateScaledBitmap bug fixed
        if (source is WriteableBitmap || source is RenderTargetBitmap)
        {
            context.DrawImage(source, new(source.Size), rect);
            return;
        }
        var scaled = GetScaledThumnail(source, rect.Size);
        var scaledRect = new Rect(scaled.Size);
        var drawRect = rect.CenterRect(scaledRect);
        context.DrawImage(scaled, scaledRect, drawRect);
    }

    private static Bitmap GetScaledThumnail(Bitmap source, Size toSize)
    {
        var toWidth = toSize.Width;
        var toHeight = toSize.Height;
        var toRatio = toSize.Width / toSize.Height;
        var sourceRatio = source.Size.Width / source.Size.Height;
        if (sourceRatio > toRatio)
        {
            toWidth = toSize.Width;
            toHeight = toWidth / sourceRatio;
        }
        else if (sourceRatio < toRatio)
        {
            toHeight = toSize.Height;
            toWidth = toHeight * sourceRatio;
        }
        return source.CreateScaledBitmap(new((int)toWidth, (int)toHeight));
    }
}
