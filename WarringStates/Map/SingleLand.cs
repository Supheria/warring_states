using LocalUtilities.TypeGeneral;

namespace WarringStates.Map;

public class SingleLand(Coordinate site, SingleLandTypes type) : Land
{
    public SingleLandTypes LandType { get; set; } = type;

    public override Coordinate Site { get; set; } = site;

    public static ColorSelector Colors { get; } = new SingleLandColors();

    public override Color Color => Colors[LandType];

    public SingleLand() : this(new(), SingleLandTypes.None)
    {

    }
}
