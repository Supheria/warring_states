using LocalUtilities.TypeGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarringStates.Map;

public abstract class LandMap
{
    public Size WorldSize { get; set; }

    public int WorldWidth => WorldSize.Width;

    public int WorldHeight => WorldSize.Height;

    public abstract Land this[Coordinate point] { get; }

    /// <summary>
    /// use <see cref="this[Coordinate]"/> to get <see cref="Land"/>, rather than use <see cref="SingleLands"/> directly
    /// </summary>
    protected LandRoster<SingleLand> SingleLands { get; } = [];

    protected LandRoster<SourceLand> SourceLands { get; } = [];

    public static Coordinate SetPointWithin(Coordinate point, Size worldSize)
    {
        var x = point.X % worldSize.Width;
        if (x < 0)
            x += worldSize.Width;
        var y = point.Y % worldSize.Height;
        if (y < 0)
            y += worldSize.Height;
        return new(x, y);
    }

    public Coordinate SetPointWithin(Coordinate point)
    {
        return SetPointWithin(point, WorldSize);
    }

    public Bitmap GetThumbnail()
    {
        var thumbnail = new Bitmap(WorldWidth, WorldHeight);
        var pThumbnail = new PointBitmap(thumbnail);
        pThumbnail.LockBits();
        for (int i = 0; i < WorldWidth; i++)
        {
            for (int j = 0; j < WorldHeight; j++)
            {
                var color = this[new(i, j)].Color;
                pThumbnail.SetPixel(i, j, color);
            }
        }
        pThumbnail.UnlockBits();
        return thumbnail;
    }
}
