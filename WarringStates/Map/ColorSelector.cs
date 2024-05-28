using LocalUtilities.SimpleScript.Serialization;

namespace WarringStates.Map;

public abstract class ColorSelector : ISsSerializable
{
    public abstract string LocalName { get; }

    protected abstract Dictionary<Enum, Color> Colors { get; }

    public Color this[Enum type] => Colors[type];

    public void SetColor(Enum type, string colorName)
    {
        Colors[type] = Color.FromName(colorName);
    }

    public void Serialize(SsSerializer serializer)
    {
        foreach (var pair in Colors)
            serializer.WriteTag(pair.Key.ToString(), pair.Value.Name);
    }

    public void Deserialize(SsDeserializer deserializer)
    {
        foreach (var pair in Colors)
            Colors[pair.Key] = deserializer.ReadTag(pair.Key.ToString(), Color.FromName);
    }
}
