using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Metadata;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using LocalUtilities.TypeToolKit.Mathematic;
using System;
using System.ComponentModel;
using WarringStates.Map;
using WarringStates.Server.GUI.ViewModels;

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

    public Color FrontColor
    {
        get => GetValue(FrontColorProperty);
        set => SetValue(FrontColorProperty, value);
    }
    public static readonly StyledProperty<Color> FrontColorProperty =
        AvaloniaProperty.Register<Thumbnail, Color>(nameof(FrontColor));

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

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        switch (change.Property.Name)
        {
            case nameof(SelectedArchive):
            //case nameof(Bounds):
                ResetSource();
                break;
        }
        //InvalidateVisual();
    }

    public sealed override void Render(DrawingContext context)
    {
        base.Render(context);
        var source = Source;
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
            toHeight = (toWidth / sourceRatio).ToRoundInt();
        }
        else if (sourceRatio < toRatio)
        {
            toHeight = toSize.Height;
            toWidth = (toHeight * sourceRatio).ToRoundInt();
        }
        return new(toWidth, toHeight);
    }

    public void ResetSource()
    {
        //if (SelectedArchive is null)
        {
            var width = (int)Bounds.Width;
            var height = (int)Bounds.Height;
            if (width <= 0 || height <= 0)
                return;
            Source?.Dispose();
            var asset = AssetLoader.Open(new("avares://WarringStates.Server.GUI/Assets/Random.bmp"));
            Source = new Bitmap(asset);
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