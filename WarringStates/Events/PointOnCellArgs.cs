using LocalUtilities.TypeGeneral;

namespace WarringStates.Events;

public sealed class PointOnCellArgs(Coordinate terrainPoint, Directions realPointOnPart)
{
    public Coordinate TerrainPoint { get; } = terrainPoint;

    public Directions PointOnCellPart { get; } = realPointOnPart;
}
