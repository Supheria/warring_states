using LocalUtilities.TypeGeneral;

namespace WarringStates.Map;

public class SingleLand(Coordinate point, LandTypes type) : Land
{
    public override LandTypes LandType { get; set; } = type;

    public override Coordinate Site { get; set; } = point;

    public static ColorSelector Colors { get; } = new SingleLandColors();

    public override Color Color => Colors[LandType];

    public SingleLand() : this(new(), LandTypes.Plain)
    {

    }
}
