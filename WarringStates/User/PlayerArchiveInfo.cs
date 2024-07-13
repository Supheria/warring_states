using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral.Convert;
using WarringStates.Map.Terrain;

namespace WarringStates.User;

public class PlayerArchiveInfo(string id, string worldName, Size worldSize, int playerCount, List<SourceLand> ownSourceLands) : ISsSerializable
{
    public string LocalName => nameof(PlayerArchiveInfo);

    public string Id { get; private set; } = id;

    public string WorldName { get; private set; } = worldName;

    public Size WorldSize { get; private set; } = worldSize;

    public int PlayerCount { get; private set; } = playerCount;

    public List<SourceLand> OwnSourceLands { get; private set; } = [];

    public PlayerArchiveInfo() : this("", "", new(), 0, [])
    {

    }

    public void Serialize(SsSerializer serializer)
    {
        serializer.WriteTag(nameof(Id), Id);
        serializer.WriteTag(nameof(WorldName), WorldName);
        serializer.WriteTag(nameof(WorldSize), WorldSize.ToArrayString());
        serializer.WriteTag(nameof(PlayerCount), PlayerCount.ToString());
        serializer.WriteObjects(nameof(OwnSourceLands), OwnSourceLands);
    }

    public void Deserialize(SsDeserializer deserializer)
    {
        Id = deserializer.ReadTag(nameof(Id));
        WorldName = deserializer.ReadTag(nameof(WorldName));
        WorldSize = deserializer.ReadTag(nameof(WorldSize), s => s.ToSize());
        PlayerCount = deserializer.ReadTag(nameof(PlayerCount), s => s.ToInt());
        OwnSourceLands = deserializer.ReadObjects<SourceLand>(nameof(OwnSourceLands));
    }
}
