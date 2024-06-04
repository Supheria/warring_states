namespace WarringStates.Events;

public class GridToRelocateArgs(Image source, Color backColor)
{
    public Image Source { get; } = source;

    public Color BackColor { get; } = backColor;
}
