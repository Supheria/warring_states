using LocalUtilities.TypeGeneral;
using WarringStates.Events;

namespace WarringStates.Client.Events;

internal class GridRedrawArgs(Image source, Rectangle drawRect, Coordinate origin) : EventArgs
{
    public Image Source { get; } = source;

    public Rectangle DrawRect { get; } = drawRect;

    public Coordinate Origin { get; } = origin;
}
