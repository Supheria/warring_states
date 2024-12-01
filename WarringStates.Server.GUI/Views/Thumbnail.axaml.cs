using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using WarringStates.Map;

namespace WarringStates.Server.GUI.Views;

public partial class Thumbnail : Control
{
    public ArchiveInfo? SelectedArchive
    {
        get => GetValue(SelectedArchiveProperty);
        set => SetValue(SelectedArchiveProperty, value);
    }
    public static readonly StyledProperty<ArchiveInfo?> SelectedArchiveProperty =
        AvaloniaProperty.Register<Thumbnail, ArchiveInfo?>(nameof(SelectedArchive));

    public Color BackColor
    {
        get => GetValue(BackColorProperty);
        set => SetValue(BackColorProperty, value);
    }
    public static readonly StyledProperty<Color> BackColorProperty =
        AvaloniaProperty.Register<Thumbnail, Color>(nameof(BackColor));

    public Thumbnail()
    {
        InitializeComponent();
        AffectsRender<Thumbnail>(SelectedArchiveProperty);
    }

    //protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    //{
    //    base.OnPropertyChanged(change);
    //    switch (change.Property.Name)
    //    {
    //        case nameof(SelectedArchive):
    //        case nameof(Bounds):
    //            ResetSource();
    //            break;
    //    }
    //}

    public sealed override void Render(DrawingContext context)
    {
        base.Render(context);
        using var source = GetThumbnail();
        var width = (int)Bounds.Width;
        var height = (int)Bounds.Height;
        if (source is null || width <= 0 || height <= 0)
            return;
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

    public Bitmap? GetThumbnail()
    {
        var width = (int)Bounds.Width;
        var height = (int)Bounds.Height;
        if (width <= 0 || height <= 0)
            return null;
        //if (SelectedArchive is null)
        {
            var source = new RenderTargetBitmap(new(width, height));
            using var context = source.CreateDrawingContext();
            context.FillRectangle(new SolidColorBrush(BackColor), new(new(width, height)));
            return source;
            //using var writer = Source.CreateDrawingContext();
            //writer.FillRectangle(new SolidColorBrush(Colors.AliceBlue), new(Bounds.Size));
            //Source = new WriteableBitmap(new(width, height), new(96, 96));
            //using var lockedBuffer = Source.Lock();
            //var random = new Random();
            //for (var i = 0; i < width; i++)
            //{
            //    for (var j = 0; j < height; j++)
            //    {
            //        Color color;
            //        if (random.Next() < random.Next())
            //            color = BackColor;
            //        else
            //            color = FrontColor;
            //        lockedBuffer.SetPixel(i, j, color);
            //    }
            //}
            //Source.Save("Random.bmp");
        }
        //Width = Bounds.Width;
        //Height = Bounds.Height;
        //drawer.FillRectangle(new SolidColorBrush(Color.FromRgb(100, 100, 100)), new(0, 0, Bounds.Width, Bounds.Height));
        //if (Background != null)
        //{
        //    var renderSize = Bounds.Size;
        //    drawer.FillRectangle(Background, new Rect(renderSize));
        //}
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
    }
}