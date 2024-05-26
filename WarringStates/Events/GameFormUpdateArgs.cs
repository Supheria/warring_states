namespace WarringStates.Events;

internal class GameFormUpdateArgs(Rectangle GameRect)
{
    public Rectangle GameRect { get; } = GameRect;
}
