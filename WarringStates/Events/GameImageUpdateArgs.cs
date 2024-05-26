using LocalUtilities.TypeGeneral;

namespace WarringStates.Events;

public class GameImageUpdateArgs(Image source, Color backColor, Coordinate originOffset)
{
    public Image Source { get; } = source;

    public Color BackColor { get; } = backColor;

    public Coordinate OriginOffset { get; } = originOffset;
}
