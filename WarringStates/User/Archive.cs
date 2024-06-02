using AltitudeMapGenerator;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeToolKit.Text;
using WarringStates.Terrain;

namespace WarringStates.User;

public class Archive : ISsSerializable
{
    public string LocalName => nameof(Archive);

    public ArchiveInfo Info { get; private set; } = new();

    public int CurrentSpan { get; private set; } = 0;

    public AltitudeMap AltitudeMap { get; private set; } = new();

    public List<SourceLand> SourceLands { get; private set; } = [];

    private Archive(ArchiveInfo info, AltitudeMap altitudeMap)
    {
        Info = info;
        AltitudeMap = altitudeMap;
    }

    public Archive()
    {

    }

    public static Archive Create(ArchiveInfo info, AltitudeMapData mapData)
    {
        var map = new AltitudeMap(mapData);
        return new(info, map);
    }

    public bool Useable()
    {
        return Info != new ArchiveInfo() && AltitudeMap.OriginPoints.Count > 0;
    }

    public void Serialize(SsSerializer serializer)
    {
        serializer.WriteObject(Info);
        serializer.WriteTag(nameof(CurrentSpan), CurrentSpan.ToString());
        serializer.WriteObject(AltitudeMap);
        serializer.WriteObjects(nameof(SourceLands), SourceLands);
    }

    public void Deserialize(SsDeserializer deserializer)
    {
        deserializer.ReadObject(Info);
        CurrentSpan = deserializer.ReadTag(nameof(CurrentSpan), int.Parse);
        deserializer.ReadObject(AltitudeMap);
        SourceLands = deserializer.ReadObjects<SourceLand>(nameof(SourceLands));
    }
}
