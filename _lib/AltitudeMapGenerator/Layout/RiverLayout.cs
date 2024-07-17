namespace AltitudeMapGenerator.Layout;

public class RiverLayout(params (RiverEndnode Start, RiverEndnode Finish)[] endnodePairs)
{
    public (RiverEndnode Start, RiverEndnode Finish) this[int index] => Layout[index];

    public List<(RiverEndnode Start, RiverEndnode Finish)> Layout { get; } = endnodePairs.ToList();

    public enum Types
    {
        Horizontal,
        Vertical,
        ForwardSlash,
        BackwardSlash,
        OneForTest
    }
}
