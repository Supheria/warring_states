using LocalUtilities.General;

namespace AltitudeMapGenerator;

public class AltitudePoint(Coordinate coordinate, double altitude)
{
    public Coordinate Coordinate { get; private set; } = coordinate;

    public double Altitude { get; set; } = altitude;

    public AltitudePoint() : this(new(), 0)
    {

    }
}
