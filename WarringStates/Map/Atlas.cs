using LocalUtilities;

namespace WarringStates.Map;

public class Atlas
{
    public static Size Size { get; protected set; } = new();

    public static int Width => Size.Width;

    public static int Height => Size.Height;

    public static Coordinate SetPointWithin(Coordinate point, Size range)
    {
        var x = point.X % range.Width;
        if (x < 0)
            x += range.Width;
        var y = point.Y % range.Height;
        if (y < 0)
            y += range.Height;
        return new(x, y);
    }

    public static Coordinate SetPointWithin(Coordinate point)
    {
        return SetPointWithin(point, Size);
    }
}
