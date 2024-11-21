using LocalUtilities.SQLiteHelper;
using LocalUtilities.TypeGeneral;
using LocalUtilities.SimpleScript;
using WarringStates.Map;

namespace WarringStates.Server.Map;

public class LandPoint(Coordinate site, SingleLandTypes landType)
{
    public Coordinate Site => new(X, Y);

    public int X { get; private set; } = site.X;

    public int Y { get; private set; } = site.Y;

    public SingleLandTypes Type { get; private set; } = landType;

    public LandPoint() : this(new(), SingleLandTypes.None)
    {

    }
}
