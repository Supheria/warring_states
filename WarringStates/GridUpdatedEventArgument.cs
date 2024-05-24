using LocalUtilities.TypeGeneral;

namespace WarringStates;

internal class GridUpdatedEventArgument(Rectangle drawRect, Coordinate origin)
{
    public Rectangle DrawRect { get; } = drawRect;

    public Coordinate Origin { get; } = origin;
}
