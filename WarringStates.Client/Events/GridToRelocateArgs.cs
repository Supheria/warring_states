using WarringStates.Events;

namespace WarringStates.Client.Events;

public class GridToRelocateArgs(Image source, Color backColor) : ICallbackArgs
{
    public Image Source { get; } = source;

    public Color BackColor { get; } = backColor;
}
