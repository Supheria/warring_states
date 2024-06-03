using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Text;

namespace WarringStates.User;

public class ArchiveInfo : ISsSerializable
{
    public static string RootPath { get; set; } = Directory.CreateDirectory(nameof(Archive)).FullName;

    public static string ThumbnailPath { get; set; } = Directory.CreateDirectory(Path.Combine(RootPath, "thumbnail")).FullName;

    public static string RegisterPath { get; } = Path.Combine(RootPath, "saves");

    public string LocalName => nameof(ArchiveInfo);

    public string Id { get; private set; } = "";

    public string WorldName { get; private set; } = "";

    public string CreateTime { get; private set; } = "";

    public string LastSaveTime { get; private set; } = "";

    public int CurrentSpan { get; private set; } = 0;

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

    public bool Useable()
    {
        return WorldName != "" && CreateTime != "" && LastSaveTime != "";
    }

    public string GetArchivePath()
    {
        return Path.Combine(RootPath, Id);
    }

    public string GetOverviewPath()
    {
        return Path.Combine(ThumbnailPath, Id);
    }

    public void Serialize(SsSerializer serializer)
    {
        serializer.WriteTag(nameof(WorldName), WorldName);
        serializer.WriteTag(nameof(CreateTime), CreateTime);
        serializer.WriteTag(nameof(LastSaveTime), LastSaveTime);
        serializer.WriteTag(nameof(CurrentSpan), CurrentSpan.ToString());
    }

    public void Deserialize(SsDeserializer deserializer)
    {
        WorldName = deserializer.ReadTag(nameof(WorldName), s => s);
        CreateTime = deserializer.ReadTag(nameof(CreateTime), s => s);
        LastSaveTime = deserializer.ReadTag(nameof(LastSaveTime), s => s);
        CurrentSpan = deserializer.ReadTag(nameof(CurrentSpan), int.Parse);
        SetId();
    }
}