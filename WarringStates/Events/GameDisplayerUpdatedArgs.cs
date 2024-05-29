namespace WarringStates.Events;

internal class GameDisplayerUpdatedArgs(Rectangle displayRect, Rectangle otherRect)
{

    public Rectangle DisplayRect { get; } = displayRect;

    public Rectangle OtherRect { get; } = otherRect;
}
