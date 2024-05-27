using LocalUtilities.SimpleScript.Serialization;

namespace WarringStates.Graph;

public class GridData : ISsSerializable
{
    public string LocalName => nameof(GridData);

    public double GuideLineWidth { get; set; } = 1.75;

    public Color GuideLineColor { get; set; } = Color.Red;

    public SolidBrush GuideLineBrush { get; } = new(Color.Transparent);

    public void Serialize(SsSerializer serializer)
    {
        serializer.WriteTag(nameof(GuideLineWidth), GuideLineWidth.ToString());
        serializer.WriteTag(nameof(GuideLineColor), GuideLineColor.Name.ToString());
    }

    public void Deserialize(SsDeserializer deserializer)
    {
        GuideLineWidth = deserializer.ReadTag(nameof(GuideLineWidth), double.Parse);
        GuideLineColor = Color.FromName(deserializer.ReadTag(nameof(GuideLineColor), s => s));
    }
}
