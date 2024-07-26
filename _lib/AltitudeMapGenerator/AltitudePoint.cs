using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Convert;

namespace AltitudeMapGenerator;

public class AltitudePoint(Coordinate coordinate, double altitude) : IArrayStringConvertable
{
    public Coordinate Coordinate { get; private set; } = coordinate;

    public double Altitude { get; set; } = altitude;

    public AltitudePoint() : this(new(), 0)
    {

    }

    public override string ToString()
    {
        return ToArrayString();
    }

    public string ToArrayString()
    {
        return ArrayString.ToArrayString(Coordinate.ToArrayString(), Altitude);
    }

    public void ParseArrayString(string str)
    {
        var array = str.ToArray();
        if (array.Length is not 3 ||
            !int.TryParse(array[0], out var x) ||
            !int.TryParse(array[1], out var y) ||
            !int.TryParse(array[2], out var altitude))
            return;
        Coordinate = new(x, y);
        Altitude = altitude;
    }
}
