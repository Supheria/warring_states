using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;

namespace WarringStates.Map;

public partial class SourceLand : ILand, ISsSerializable
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
        public override string LocalName => nameof(SourceLandColors);

        protected override Dictionary<Enum, Color> Colors { get; } = new()
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

    public Dictionary<Product.Types, Product> Products { get; } = [];

    public Enum Type { get; private set; }

    public Color Color { get; private set; }

    Dictionary<Coordinate, Directions> Points { get; set; }

    private SourceLand(Dictionary<Coordinate, Directions> points, Types type, List<Product> products)
    {
        Type = type;
        Color = Colors[Type];
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

    public void Serialize(SsSerializer serializer)
    {
        serializer.WriteTag(nameof(Type), Type.ToString());
        serializer.WriteValues(nameof(Points), Points.ToList(), c => c.ToString());
        serializer.WriteObjects(nameof(Products), Products.Values);
    }

    public void Deserialize(SsDeserializer deserializer)
    {
        throw new NotImplementedException();
    }
}
