using LocalUtilities.TypeGeneral;
using WarringStates.Events;

namespace WarringStates.Client.Events;

internal class GridRedrawArgs(Bitmap source, Rectangle drawRect, Coordinate origin) : ICallbackArgs
{
    public Bitmap Source { get; } = source;

    public Rectangle DrawRect { get; } = drawRect;

    public Coordinate Origin { get; } = origin;
}
