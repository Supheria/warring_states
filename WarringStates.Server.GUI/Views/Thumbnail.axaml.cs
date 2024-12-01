using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using WarringStates.Map;
using WarringStates.Server.GUI.Models;

namespace WarringStates.Server.GUI.Views;

public partial class Thumbnail : UserControl
{
    public Color BackColor
    {
        get => GetValue(BackColorProperty);
        set => SetValue(BackColorProperty, value);
    }
    public static readonly StyledProperty<Color> BackColorProperty =
        AvaloniaProperty.Register<Thumbnail, Color>(nameof(BackColor));

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
        var width = (int)Bounds.Width;
        var height = (int)Bounds.Height;
        if (width <= 0 || height <= 0)
            return;
        var source = Source;
        if (source is null)
        {
            context.FillRectangle(new SolidColorBrush(BackColor), new(new(width, height)));
            return;
        }
        var scaledSize = ScaleInRatio(source.Size, Bounds.Size);
        var drawRect = new Rect(Bounds.Size).CenterRect(new(scaledSize));
        context.DrawImage(source, new(source.Size), drawRect);
    }

    private static Size ScaleInRatio(Size fromSize, Size toSize)
    {
        var toWidth = toSize.Width;
        var toHeight = toSize.Height;
        var toRatio = toSize.Width / toSize.Height;
        var sourceRatio = fromSize.Width / fromSize.Height;
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
        return new(toWidth, toHeight);
    }
}
