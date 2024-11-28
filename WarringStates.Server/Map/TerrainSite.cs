using LocalUtilities.TypeGeneral;
using WarringStates.Map;

namespace WarringStates.Server.Map;

public class TerrainSite(Coordinate site, SingleLandTypes landType)
{
    public Coordinate Site => new(X, Y);

    public int X { get; private set; } = site.X;

    public int Y { get; private set; } = site.Y;

    public SingleLandTypes Type { get; private set; } = landType;

    public TerrainSite() : this(new(), SingleLandTypes.None)
    {

    }
}
