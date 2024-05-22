using LocalUtilities.SimpleScript.Serialization;

namespace WarringStates;

using Type = Terrain.Type;
public class TerrainOverviewColors : ISsSerializable
{
    public string LocalName { get; set; } = nameof(TerrainOverviewColors);

    public static int Alpha { get; set; } = 233;

    Dictionary<Type, Color> ColorMap { get; } = new()
    {
        [Type.Plain] = Color.LightYellow,
        [Type.Woodland] = Color.ForestGreen,
        [Type.Stream] = Color.SkyBlue,
        [Type.Hill] = Color.DarkSlateGray,
    };

    public Color this[Type type] => Color.FromArgb(Alpha, ColorMap[type]);

    public void SetColor(Type terrain, string colorName)
    {
        ColorMap[terrain] = Color.FromName(colorName);
    }

    public void Serialize(SsSerializer serializer)
    {
        serializer.WriteTag(nameof(Alpha), Alpha.ToString());
        foreach (var pair in ColorMap)
            serializer.WriteTag(pair.Key.ToString(), pair.Value.Name);
    }

    public void Deserialize(SsDeserializer deserializer)
    {
        Alpha = deserializer.ReadTag(nameof(Alpha), int.Parse);
        foreach (var pair in ColorMap)
            ColorMap[pair.Key] = deserializer.ReadTag(pair.Key.ToString(), Color.FromName);
    }
}
