using LocalUtilities.TypeGeneral;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.DataFormats;

namespace WarringStates.UI;

public class CBitmap(nint data, int width, int height, int channels, int stride) : IDisposable
{
    public nint Buffer { get; } = data;

    public Bitmap Source { get; } = channels switch
    {
        1 => new Bitmap(width, height, stride, PixelFormat.Format8bppIndexed, data),
        3 => new Bitmap(width, height, stride, PixelFormat.Format24bppRgb, data),
        4 => new Bitmap(width, height, stride, PixelFormat.Format32bppArgb, data),
        _ => throw new ArgumentException("channels of bitmap is out range"),
    };

    public void Dispose()
    {
        //this.ReleaseImage();
        GC.SuppressFinalize(this);
    }
}
