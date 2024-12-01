using LocalUtilities;
using LocalUtilities.General;

namespace AltitudeMapGenerator.DLA;

internal class DlaPixel(int x, int y)
{
    internal int X => x;

    internal int Y => y;

    internal Dictionary<Directions, Coordinate> Neighbor { get; } = [];

    internal Dictionary<Directions, int> ConnetNumber { get; } = [];

    internal double Altitude
    {
        get
        {
            if (_altitude is -1)
            {
                if (!ConnetNumber.TryGetValue(Directions.Top, out var top))
                    top = 0;
                if (!ConnetNumber.TryGetValue(Directions.Bottom, out var bottom))
                    bottom = 0;
                if (!ConnetNumber.TryGetValue(Directions.Left, out var left))
                    left = 0;
                if (!ConnetNumber.TryGetValue(Directions.Right, out var right))
                    right = 0;
                if (!ConnetNumber.TryGetValue(Directions.LeftTop, out var leftTop))
                    leftTop = 0;
                if (!ConnetNumber.TryGetValue(Directions.BottomRight, out var bottomRight))
                    bottomRight = 0;
                if (!ConnetNumber.TryGetValue(Directions.TopRight, out var topRight))
                    topRight = 0;
                if (!ConnetNumber.TryGetValue(Directions.LeftBottom, out var leftBottom))
                    leftBottom = 0;
                var list = new List<int>()
                {
                    top + bottom - Math.Abs(top - bottom),
                    left + right - Math.Abs(left - right),
                    leftTop + bottomRight - Math.Abs(leftTop - bottomRight),
                    topRight + leftBottom - Math.Abs(topRight - leftBottom),
                };
                _altitude = list.Max();
            }
            return _altitude;
        }
        set => _altitude = value;
    }
    double _altitude = -1;

    internal DlaPixel() : this(0, 0)
    {

    }

    public static implicit operator Coordinate(DlaPixel pixel)
    {
        return new(pixel.X, pixel.Y);
    }

}
