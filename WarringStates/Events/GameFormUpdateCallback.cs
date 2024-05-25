namespace WarringStates.Events;

internal class GameFormUpdateCallback(Size clientSize)
{
    public Size ClientSize { get; } = clientSize;
}
