using LocalUtilities.SimpleScript.Serialization;

namespace WarringStates;

using Type = Terrain.Type;
public class TerrainColors : ISsSerializable
{
    public string LocalName { get; set; } = nameof(TerrainColors);

    Dictionary<Type, Color> ColorMap { get; } = new()
    {
        [Type.Plain] = Color.LightYellow,
        [Type.Woodland] = Color.ForestGreen,
        [Type.Stream] = Color.SkyBlue,
        [Type.Hill] = Color.DarkSlateGray,
    };

    public Color this[Type type] => ColorMap[type];

    public void SetColor(Type terrain, string colorName)
    {
        ColorMap[terrain] = Color.FromName(colorName);
    }

    public void Serialize(SsSerializer serializer)
    {
        foreach (var pair in ColorMap)
            serializer.WriteTag(pair.Key.ToString(), pair.Value.Name);
    }

    public void Deserialize(SsDeserializer deserializer)
    {
        foreach (var pair in ColorMap)
            ColorMap[pair.Key] = deserializer.ReadTag(pair.Key.ToString(), Color.FromName);
    }
}
