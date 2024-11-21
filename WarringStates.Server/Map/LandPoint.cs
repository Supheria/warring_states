using LocalUtilities.SQLiteHelper;
using LocalUtilities.TypeGeneral;
using LocalUtilities.SimpleScript;

namespace WarringStates.Server.Map;

public class LandPoint(Coordinate coordinate, double altitudeRatio, PointTypes type)
{
    [SsItem(Name = "c")]
    public Coordinate Coordinate { get; private set; } = coordinate;

    [SsItem(Name = "a")]
    public double AltitudeRatio { get; private set; } = altitudeRatio;

    [SsItem(Name = "t")]
    public PointTypes Type { get; private set; } = type;

    public LandPoint() : this(new(), 0, PointTypes.Normal)
    {

    }
}
