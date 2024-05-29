using LocalUtilities.TypeGeneral;

namespace WarringStates.Events;

public class GridImageToUpdateArgs(Image source, Color backColor)
{
    public Image Source { get; } = source;

    public Color BackColor { get; } = backColor;
}
