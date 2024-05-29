namespace WarringStates.Events;

public class GameFormUpdatedArgs(Rectangle GameRect)
{
    public Rectangle GameRect { get; } = GameRect;
}
