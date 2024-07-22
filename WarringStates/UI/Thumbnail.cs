using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Graph;
using LocalUtilities.TypeToolKit.Mathematic;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Flow;
using WarringStates.Flow.Model;
using WarringStates.User;

namespace WarringStates.UI;

public class Thumbnail : Displayer
{
    Bitmap? ThumbnailSource { get; set; } = null;

    int CurrentSpan { get; set; } = 0;

    public override void Redraw()
    {
        using var g = Graphics.FromImage(Image);
        if (ThumbnailSource is null)
        {
            var random = new Random();
            var pImage = new PointBitmap((Bitmap)Image);
            pImage.LockBits();
            for (var i = 0; i < Width; i++)
            {
                for (var j = 0; j < Height; j++)
                {
                    Color color;
                    if (random.Next() < random.Next())
                        color = BackColor;
                    else
                        color = FrontColor;
                    pImage.SetPixel(i, j, color);
                }
            }
            pImage.UnlockBits();
        }
        else
        {
            g.Clear(FrontColor);
            var rect = new Rectangle();
            try
            {
                var size = GeometryTool.ScaleSizeWithinRatio(ThumbnailSource.Size, Size);
                var thumbnail = BitmapTool.CopyToNewSize(ThumbnailSource, size, InterpolationMode.Low);
                rect = new((Width - thumbnail.Width) / 2, 0, thumbnail.Width, thumbnail.Height);
                BitmapTool.DrawTemplateOnto(thumbnail, (Bitmap)Image, rect, true);
                rect = new(0, thumbnail.Height, Width, Height - thumbnail.Height);
                thumbnail.Dispose();
            }
            catch { }
            var stepper = new DateStepper();
            stepper.SetStartSpan(CurrentSpan);
            var format = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            g.DrawString(stepper.GetDate().ToString(), LabelFontData, new SolidBrush(Color.Black), rect, format);
        }
    }

    public void SetThumbnail(Bitmap thumbnail, int currentSpan)
    {
        ThumbnailSource?.Dispose();
        ThumbnailSource = thumbnail;
        CurrentSpan = currentSpan;
    }

    public void SetThumbnailVoid()
    {
        ThumbnailSource?.Dispose();
        ThumbnailSource = null;
        CurrentSpan = 0;
    }
}
