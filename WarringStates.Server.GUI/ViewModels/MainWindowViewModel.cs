using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace WarringStates.Server.GUI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public ThumbnailViewModel ThumbnailViewModel { get; } = new();

    [ObservableProperty]
    List<string> _archiveList = ["存档一", "存档二"];

    [ObservableProperty]
    RenderTargetBitmap _ThumbnailSource;

    //private RenderTargetBitmap GetThumbnail()
    //{
    //    base.Redraw();
    //    using var g = Graphics.FromImage(Image);
    //    if (ThumbnailSource is null)
    //    {
    //        var random = new Random();
    //        var pImage = new PointBitmap((Bitmap)Image);
    //        pImage.LockBits();
    //        for (var i = 0; i < ClientWidth; i++)
    //        {
    //            for (var j = 0; j < ClientHeight; j++)
    //            {
    //                Color color;
    //                if (random.Next() < random.Next())
    //                    color = BackColor;
    //                else
    //                    color = FrontColor;
    //                pImage.SetPixel(i, j, color);
    //            }
    //        }
    //        pImage.UnlockBits();
    //    }
    //    else
    //    {
    //        g.Clear(FrontColor);
    //        var rect = new Rectangle();
    //        try
    //        {
    //            var size = GeometryTool.ScaleSizeWithinRatio(ThumbnailSource.Size, ClientSize);
    //            var thumbnail = BitmapTool.CopyToNewSize(ThumbnailSource, size, InterpolationMode.Low);
    //            rect = new((ClientWidth - thumbnail.Width) / 2, 0, thumbnail.Width, thumbnail.Height);
    //            BitmapTool.DrawTemplateOnto(thumbnail, (Bitmap)Image, rect, true);
    //            rect = new(0, thumbnail.Height, Width, Height - thumbnail.Height);
    //            thumbnail.Dispose();
    //        }
    //        catch { }
    //        var stepper = new DateStepper();
    //        stepper.SetStartSpan(CurrentSpan);
    //        var format = new StringFormat()
    //        {
    //            Alignment = StringAlignment.Center,
    //            LineAlignment = StringAlignment.Center
    //        };
    //        g.DrawString(stepper.GetDate().ToString(), LabelFontData, new SolidBrush(Color.Black), rect, format);
    //    }
    //}
}
