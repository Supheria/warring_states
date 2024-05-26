namespace WarringStates.Events;

internal class GameDisplayerUpdateArgs(Rectangle otherRect)
{
    public Rectangle OtherRect { get; } = otherRect;
}
