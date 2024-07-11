using LocalUtilities.TypeGeneral;

namespace WarringStates.Client.Events;

internal class GridRelocatedArgs(Rectangle drawRect, Coordinate origin)
{
    public Rectangle DrawRect { get; } = drawRect;

    public Coordinate Origin { get; } = origin;
}
