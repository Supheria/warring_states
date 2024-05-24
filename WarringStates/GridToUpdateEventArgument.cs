using LocalUtilities.TypeGeneral;

namespace WarringStates;

public class GridToUpdateEventArgument(Image source, Rectangle drawRect, Color backColor, Coordinate originOffset)
{
    public Image Source { get; } = source;

    public Rectangle DrawRect { get; } = drawRect;

    public Color BackColor { get; } = backColor;

    public Coordinate OriginOffset { get; } = originOffset;

    public GridToUpdateEventArgument(Image source, Rectangle drawRect, Color backColor) : this(source, drawRect, backColor, new())
    {

    }
}
