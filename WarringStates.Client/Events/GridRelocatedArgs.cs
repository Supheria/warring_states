using LocalUtilities.TypeGeneral;
using WarringStates.Events;

namespace WarringStates.Client.Events;

internal class GridRelocatedArgs(Rectangle drawRect, Coordinate origin) : ICallbackArgs
{
    public Rectangle DrawRect { get; } = drawRect;

    public Coordinate Origin { get; } = origin;
}
