using LocalUtilities.SQLiteHelper;
using LocalUtilities.TypeGeneral;

namespace WarringStates.Map;

public abstract class Land : IRosterItem<Coordinate>
{
    public abstract LandTypes LandType { get; set; }

    public abstract Coordinate Site { get; set; }

    public abstract Color Color { get; }

    public Coordinate Signature => Site;
}
