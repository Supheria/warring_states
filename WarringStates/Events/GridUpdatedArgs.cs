using LocalUtilities.TypeGeneral;

namespace WarringStates.Events;

internal class GridUpdatedArgs(Rectangle drawRect, Coordinate origin, Bitmap bitmap)
{
    public Rectangle DrawRect { get; } = drawRect;

    public Coordinate Origin { get; } = origin;

    public Bitmap Bitmap { get; } = bitmap;
}
