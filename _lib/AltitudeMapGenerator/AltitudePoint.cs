using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Convert;

namespace AltitudeMapGenerator;

public class AltitudePoint(int x, int y, double altitude) : IArrayStringConvertable
{
    public int X { get; private set; } = x;

    public int Y { get; private set; } = y;

    public double Altitude { get; private set; } = altitude;

    public AltitudePoint() : this(0, 0, 0)
    {

    }

    public override string ToString()
    {
        return ToArrayString();
    }

    public string ToArrayString()
    {
        return ArrayString.ToArrayString(X, Y, Altitude);
    }

    public void ParseArrayString(string str)
    {
        var array = str.ToArray();
        if (array.Length is not 3 ||
            !int.TryParse(array[0], out var x) ||
            !int.TryParse(array[1], out var y) ||
            !int.TryParse(array[2], out var altitude))
            return;
        X = x;
        Y = y;
        Altitude = altitude;
    }

    public static implicit operator Coordinate(AltitudePoint point)
    {
        return new(point.X, point.Y);
    }
}
