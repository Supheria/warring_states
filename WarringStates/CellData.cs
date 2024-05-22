using LocalUtilities.SimpleScript.Serialization;

namespace WarringStates;

public class CellData : ISsSerializable
{
    public string LocalName { get; set; } = nameof(CellData);

    public int EdgeLength
    {
        get => _edgeLength;
        set => _edgeLength = value < EdgeLengthMin || value > EdgeLengthMax ? _edgeLength : value;
    }
    int _edgeLength = 30;

    public int EdgeLengthMin { get; set; } = 25;

    public int EdgeLengthMax { get; set; } = 125;

    public double CenterPaddingFactor
    {
        get => _centerPaddingFactor;
        set => _centerPaddingFactor = value < CenterPaddingFactorMin || value > CenterPaddingFactorMax ? _centerPaddingFactor : value;
    }
    double _centerPaddingFactor = 0.333;

    public double CenterPaddingFactorMin { get; set; } = 0.01;

    public double CenterPaddingFactorMax { get; set; } = 0.4;

    public void Serialize(SsSerializer serializer)
    {
        serializer.WriteTag(nameof(EdgeLengthMin), EdgeLengthMin.ToString());
        serializer.WriteTag(nameof(EdgeLengthMax), EdgeLengthMax.ToString());
        serializer.WriteTag(nameof(EdgeLength), EdgeLength.ToString());
        serializer.WriteTag(nameof(CenterPaddingFactorMin), CenterPaddingFactorMin.ToString());
        serializer.WriteTag(nameof(CenterPaddingFactorMax), CenterPaddingFactorMax.ToString());
    }

    public void Deserialize(SsDeserializer deserializer)
    {
        EdgeLengthMin = deserializer.ReadTag(nameof(EdgeLengthMin), int.Parse);
        EdgeLengthMax = deserializer.ReadTag(nameof(EdgeLengthMax), int.Parse);
        EdgeLength = deserializer.ReadTag(nameof(EdgeLength), int.Parse);
        CenterPaddingFactorMin = deserializer.ReadTag(nameof(CenterPaddingFactorMin), float.Parse);
        CenterPaddingFactorMax = deserializer.ReadTag(nameof(CenterPaddingFactorMax), float.Parse);
        CenterPaddingFactor = deserializer.ReadTag(nameof(CenterPaddingFactor), double.Parse);
    }
}
