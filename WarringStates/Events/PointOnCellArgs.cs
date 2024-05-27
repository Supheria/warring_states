using LocalUtilities.TypeGeneral;

namespace WarringStates.Events;

public sealed class PointOnCellArgs(Coordinate terrainPoint, Direction realPointOnPart)
{
    public Coordinate TerrainPoint { get; } = terrainPoint;

    public Direction PointOnCellPart { get; } = realPointOnPart;
}
