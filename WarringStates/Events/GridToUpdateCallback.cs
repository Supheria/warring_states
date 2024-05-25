using LocalUtilities.TypeGeneral;

namespace WarringStates.Events;

public class GridToUpdateCallback(Image source, Rectangle drawRect, Color backColor, Coordinate originOffset)
{
    public Image Source { get; } = source;

    public Rectangle DrawRect { get; } = drawRect;

    public Color BackColor { get; } = backColor;

    public Coordinate OriginOffset { get; } = originOffset;

    public GridToUpdateCallback(Image source, Rectangle drawRect, Color backColor) : this(source, drawRect, backColor, new())
    {

    }
}
