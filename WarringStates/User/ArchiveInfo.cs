using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeToolKit.Text;

namespace WarringStates.User;

public class ArchiveInfo : ISsSerializable
{
    public string LocalName => nameof(ArchiveInfo);

    public string Id { get; private set; } = "";

    public string WorldName { get; private set; } = "";

    public string CreateTime { get; private set; } = "";

    public string LastSaveTime { get; private set; } = "";

    public ArchiveInfo(string worldName)
    {
        WorldName = worldName;
        LastSaveTime = CreateTime = $"{DateTime.Now:yyyyMMddHHmmss}";
        SetId();
    }

    public ArchiveInfo()
    {

    }

    private void SetId()
    {
        Id = $"{WorldName}{CreateTime}".ToMd5HashString();
    }

    public void UpdateLastSaveTime()
    {
        LastSaveTime = $"{DateTime.Now:yyyyMMddHHmmss}"; ;
    }

    public void Serialize(SsSerializer serializer)
    {
        serializer.WriteTag(nameof(WorldName), WorldName);
        serializer.WriteTag(nameof(CreateTime), CreateTime);
        serializer.WriteTag(nameof(LastSaveTime), LastSaveTime);
    }

    public void Deserialize(SsDeserializer deserializer)
    {
        WorldName = deserializer.ReadTag(nameof(WorldName), s => s);
        CreateTime = deserializer.ReadTag(nameof(CreateTime), s => s);
        LastSaveTime = deserializer.ReadTag(nameof(LastSaveTime), s => s);
        SetId();
    }
}