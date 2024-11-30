using LocalUtilities;

namespace WarringStates.Map;

public abstract class Land : IRosterItem<Coordinate>
{
    public abstract Coordinate Site { get; set; }

    public abstract Color Color { get; }

    public Coordinate Signature => Site;
}
