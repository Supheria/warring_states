using LocalUtilities.TypeGeneral;
using WarringStates.Events;

namespace WarringStates.Client.Events;

public sealed class GridCellPointedOnArgs(Coordinate terrainPoint, Directions realPointOnPart) : ICallbackArgs
{
    public Coordinate TerrainPoint { get; } = terrainPoint;

    public Directions PointOnCellPart { get; } = realPointOnPart;
}
