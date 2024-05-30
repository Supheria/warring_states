using AltitudeMapGenerator;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Flow;
using WarringStates.Graph;
using WarringStates.Terrain;

namespace WarringStates.User;

public class Archive : ISsSerializable
{
    public string LocalName => nameof(Archive);

    public string Id { get; private set; } = "";

    public string WorldName { get; private set; } = "";

    public string CreateTime { get; private set; } = "";

    public int CurrentSpan { get; private set; } = 0;

    public AltitudeMap AltitudeMap { get; private set; } = new();

    public List<SourceLand> SourceLands { get; private set; } = [];

    private Archive(string worldName, AltitudeMap altitudeMap, string createTime)
    {
        WorldName = worldName;
        AltitudeMap = altitudeMap;
        CreateTime = createTime;
        SetId();
    }

    public Archive()
    {

    }

    private void SetId()
    {
        Id = $"{WorldName}{CreateTime}".ToMd5HashString();
    }

    public static Archive Create(string worldName, AltitudeMapData mapData)
    {
        var map = new AltitudeMap(mapData);
        var date = $"{DateTime.Now:yyyyMMddHHmmss}";
        return new(worldName, map, date);
    }

    public void Serialize(SsSerializer serializer)
    {
        serializer.WriteTag(nameof(WorldName), WorldName);
        serializer.WriteTag(nameof(CreateTime), CreateTime);
        serializer.WriteObject(AltitudeMap);
        serializer.WriteObjects(nameof(SourceLands), SourceLands);
    }

    public void Deserialize(SsDeserializer deserializer)
    {
        WorldName = deserializer.ReadTag(nameof(WorldName), s => s);
        CreateTime = deserializer.ReadTag(nameof(CreateTime), s => s);
        deserializer.ReadObject(AltitudeMap);
        SourceLands = deserializer.ReadObjects<SourceLand>(nameof(SourceLands));
        SetId();
    }
}
