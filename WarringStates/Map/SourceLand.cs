using LocalUtilities.TypeGeneral;

namespace WarringStates.Map;

public class SourceLand(Coordinate site, Directions direction, SourceLandTypes type/*, List<Product> products*/) : Land
{
    public SourceLandTypes LandType { get; set; } = type;

    //public Dictionary<Product.Types, Product> Products { get; } = [];

    public override Coordinate Site { get; set; } = site;

    public static ColorSelector Colors { get; } = new SourceLandColors();

    public override Color Color => Colors[LandType];

    public Directions Direction { get; set; } = direction;

    public SourceLand() : this(new(), Directions.None, SourceLandTypes.None)
    {

    }
}
