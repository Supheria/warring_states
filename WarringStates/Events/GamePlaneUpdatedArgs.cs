namespace WarringStates.Events;

internal class GamePlaneUpdatedArgs(Rectangle displayRect, Rectangle otherRect)
{

    public Rectangle DisplayRect { get; } = displayRect;

    public Rectangle OtherRect { get; } = otherRect;
}
