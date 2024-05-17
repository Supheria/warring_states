using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarringStates;

internal class GameDisplayer : PictureBox
{
    public void ResetImage()
    {
        Image?.Dispose();
        Image = new Bitmap(Width, Height);
        var g = Graphics.FromImage(Image);
        g.Clear(Color.Black);
    }
}
