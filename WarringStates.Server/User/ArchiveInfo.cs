using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral.Convert;
using LocalUtilities.TypeToolKit.Text;
using WarringStates.User;

namespace WarringStates.Server.User;

internal class ArchiveInfo : ISsSerializable
{
    public string Id { get; private set; } = "";

    public string RootPath { get; private set; } = "";

    public string LocalName => nameof(ArchiveInfo);

    public string WorldName { get; private set; } = "";

    public long CreateTime { get; private set; } = 0;

    public long LastSaveTime { get; private set; } = 0;

    public int CurrentSpan { get; private set; } = 0;

    public ArchiveInfo(string worldName)
    {
        WorldName = worldName;
        LastSaveTime = CreateTime = DateTime.Now.ToBinary();
        Id = (WorldName + CreateTime).ToMd5HashString();
        RootPath = Path.Combine(LocalArchives.RootPath, Id);
    }

    public ArchiveInfo()
    {

    }

    public void Serialize(SsSerializer serializer)
    {
        serializer.WriteTag(nameof(WorldName), WorldName);
        serializer.WriteTag(nameof(CreateTime), CreateTime.ToString());
        serializer.WriteTag(nameof(LastSaveTime), LastSaveTime.ToString());
        serializer.WriteTag(nameof(CurrentSpan), CurrentSpan.ToString());
    }

    public void Deserialize(SsDeserializer deserializer)
    {
        WorldName = deserializer.ReadTag(nameof(WorldName));
        CreateTime = deserializer.ReadTag(nameof(CreateTime), s => s.ToLong());
        LastSaveTime = deserializer.ReadTag(nameof(LastSaveTime), s => s.ToLong());
        CurrentSpan = deserializer.ReadTag(nameof(CurrentSpan), s => s.ToInt());
        Id = (WorldName + CreateTime).ToMd5HashString();
        RootPath = Path.Combine(LocalArchives.RootPath, Id);
    }

    public void UpdateLastSaveTime()
    {
        LastSaveTime = DateTime.Now.ToBinary();
    }

    public string GetArchiveInfoPath()
    {
        return Path.Combine(RootPath, "info");
    }

    public string GetThumbnailPath()
    {
        return Path.Combine(RootPath, "thumbnail");
    }

    public string GetAltitudeMapPath()
    {
        return Path.Combine(RootPath, "altitude map");
    }

    public string GetSourceLandsPath()
    {
        return Path.Combine(RootPath, "source lands");
    }

    public string GetPlayersPath()
    {
        return Path.Combine(RootPath, "players");
    }
}
