namespace WarringStates.Events;

public class GameFormUpdateArgs(Rectangle GameRect)
{
    public Rectangle GameRect { get; } = GameRect;
}
