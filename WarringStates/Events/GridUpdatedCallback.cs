using LocalUtilities.TypeGeneral;

namespace WarringStates.Events;

internal class GridUpdatedCallback(Rectangle drawRect, Coordinate origin)
{
    public Rectangle DrawRect { get; } = drawRect;

    public Coordinate Origin { get; } = origin;
}
