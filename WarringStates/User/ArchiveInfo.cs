//using LocalUtilities.SimpleScript.Serialization;
//using LocalUtilities.TypeGeneral.Convert;
//using LocalUtilities.TypeToolKit.Text;

//namespace WarringStates.User;

//public class ArchiveInfo : ISsSerializable
//{
//    public static string RootPath { get; set; } = Directory.CreateDirectory(nameof(Archive)).FullName;

//    public static string ThumbnailPath { get; set; } = Directory.CreateDirectory(Path.Combine(RootPath, "thumbnail")).FullName;

//    public string LocalName => nameof(ArchiveInfo);

//    public string WorldName { get; private set; } = "";

//    public long CreateTime { get; private set; } = 0;

//    public long LastSaveTime { get; private set; } = 0;

//    public int CurrentSpan { get; private set; } = 0;

//    public ArchiveInfo(string worldName)
//    {
//        WorldName = worldName;
//        LastSaveTime = CreateTime = DateTime.Now.ToBinary();
//    }

//    public ArchiveInfo()
//    {

//    }

//    public void UpdateLastSaveTime()
//    {
//        LastSaveTime = DateTime.Now.ToBinary();
//    }

//    public bool Useable()
//    {
//        return WorldName != "" && CreateTime != 0 && LastSaveTime != 0 && File.Exists(GetArchivePath());
//    }

//    public string GetId()
//    {
//        return (WorldName + CreateTime).ToMd5HashString();
//    }

//    public string GetArchivePath()
//    {
//        return Path.Combine(RootPath, GetId());
//    }

//    public string GetThumbnailPath()
//    {
//        return Path.Combine(ThumbnailPath, GetId());
//    }

//    public void Serialize(SsSerializer serializer)
//    {
//        serializer.WriteTag(nameof(WorldName), WorldName);
//        serializer.WriteTag(nameof(CreateTime), CreateTime.ToString());
//        serializer.WriteTag(nameof(LastSaveTime), LastSaveTime.ToString());
//        serializer.WriteTag(nameof(CurrentSpan), CurrentSpan.ToString());
//    }

//    public void Deserialize(SsDeserializer deserializer)
//    {
//        WorldName = deserializer.ReadTag(nameof(WorldName));
//        CreateTime = deserializer.ReadTag(nameof(CreateTime), s => s.ToLong());
//        LastSaveTime = deserializer.ReadTag(nameof(LastSaveTime), s => s.ToLong());
//        CurrentSpan = deserializer.ReadTag(nameof(CurrentSpan), s => s.ToInt());
//    }
//}