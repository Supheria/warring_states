using AltitudeMapGenerator;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeToolKit.Text;
using WarringStates.Terrain;

namespace WarringStates.User;

public class Archive : ISsSerializable
{
    public string LocalName => nameof(Archive);

    public ArchiveInfo ArchiveInfo { get; private set; } = new();

    public int CurrentSpan { get; private set; } = 0;

    public AltitudeMap AltitudeMap { get; private set; } = new();

    public List<SourceLand> SourceLands { get; private set; } = [];

    private Archive(ArchiveInfo info, AltitudeMap altitudeMap)
    {
        ArchiveInfo = info;
        AltitudeMap = altitudeMap;
    }

    public Archive()
    {

    }

    public static Archive Create(string worldName, AltitudeMapData mapData)
    {
        var map = new AltitudeMap(mapData);
        return new(new(worldName), map);
    }

    public void UpdateSaveTime()
    {

    }

    public void Serialize(SsSerializer serializer)
    {
        serializer.WriteObject(ArchiveInfo);
        serializer.WriteObject(AltitudeMap);
        serializer.WriteObjects(nameof(SourceLands), SourceLands);
    }

    public void Deserialize(SsDeserializer deserializer)
    {
        deserializer.ReadObject(ArchiveInfo);
        deserializer.ReadObject(AltitudeMap);
        SourceLands = deserializer.ReadObjects<SourceLand>(nameof(SourceLands));
    }
}
