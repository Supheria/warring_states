using LocalUtilities.TypeGeneral;

namespace WarringStates.Map.Terrain;

public class SourceLand : ILand
{
    public string LocalName => nameof(SourceLand);

    public enum Types
    {
        None,
        HorseLand,
        MineLand,
        FarmLand,
        MulberryLand,
        WoodLand,
        FishLand,
        TerraceLand
    }

    public class SourceLandColors : ColorSelector
    {
        protected override Dictionary<Enum, Color> Colors { get; set; } = new()
        {
            [Types.HorseLand] = Color.LightSalmon,
            [Types.MineLand] = Color.DarkSlateGray,
            [Types.FarmLand] = Color.Gold,
            [Types.MulberryLand] = Color.MediumPurple,
            [Types.WoodLand] = Color.ForestGreen,
            [Types.FishLand] = Color.RoyalBlue,
            [Types.TerraceLand] = Color.YellowGreen,
        };
    }

    public static SourceLandColors Colors { get; set; } = new();

    public Enum Type { get; private set; }

    public Dictionary<Product.Types, Product> Products { get; } = [];

    Dictionary<Coordinate, Directions> Points { get; set; }

    public Color Color => Colors[Type];

    public SourceLand(Dictionary<Coordinate, Directions> points, Types type, List<Product> products)
    {
        Type = type;
        Points = points;
        products.ForEach(p => Products[p.Type] = p);
    }

    public SourceLand() : this([], Types.None, [])
    {

    }

    public Directions this[Coordinate point] => Points[point];

    public List<Coordinate> GetPoints()
    {
        return Points.Keys.ToList();
    }
}
