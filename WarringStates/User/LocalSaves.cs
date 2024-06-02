using AltitudeMapGenerator;
using LocalUtilities.SimpleScript.Serialization;
using System.Diagnostics.CodeAnalysis;

namespace WarringStates.User;

public static class LocalSaves
{
    public static string Folder { get; set; } = Directory.CreateDirectory(nameof(Archive)).FullName;

    public static string ArchivesDir { get; } = Path.Combine(Folder, nameof(Saves));

    public static List<ArchiveInfo> Saves { get; set; } =
        //[
        //new("测试存档中文版1"),
        //];
        //[];
        [
        new("测试存档中文版1"),
        new("测试存档中文版2"),
        new("测试存档中文版3"),
        new("测试存档中文版4"),
        new("测试存档中文版5"),
        new("测试存档中文版6"),
        new("测试存档中文版7"),
        new("测试存档中文版8"),
        new("测试存档中文版9"),
        ];

    //public static void ReLocate()
    //{
    //    try
    //    {
    //        Saves = SerializeTool.LoadFromSimpleScript<ArchiveInfo>(nameof(Saves), ArchivesDir).ToList();
    //    }
    //    catch
    //    {
    //        Save();
    //    }
    //}

    public static bool LoadArchive(this ArchiveInfo info, [NotNullWhen(true)]out Archive? archive)
    {
        archive = null;
        var dir = GetArchiveDir(info);
        if (!File.Exists(dir))
        {
            MessageBox.Show($"找不到存档文件 - {info.WorldName}@{info.CreateTime}");
            return false;
        }
        try
        {
            archive = new Archive().LoadFromSimpleScript(dir);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"存档文件 - {info.WorldName}@{info.CreateTime} 或已损坏: {ex.Message}");
            return false;
        }
    }

    public static ArchiveInfo CreateArchive(this AltitudeMapData mapData, string worldName)
    {
        var archive = Archive.Create(worldName, mapData);
        archive.SaveToSimpleScript(false, GetArchiveDir(archive.ArchiveInfo));
        Saves.Add(archive.ArchiveInfo);
        Save();
        return archive.ArchiveInfo;
    }

    public static void Update(this Archive archive)
    {
        Saves.Remove(archive.ArchiveInfo);
        archive.ArchiveInfo.UpdateLastSaveTime();
        archive.SaveToSimpleScript(false, GetArchiveDir(archive.ArchiveInfo));
        var saves = new List<ArchiveInfo>() { archive.ArchiveInfo };
        saves.AddRange(Saves);
        Saves = saves;
    }

    public static void Delete(this ArchiveInfo info)
    {
        var dir = GetArchiveDir(info);
        if (File.Exists(dir))
            File.Delete(dir);
        Saves.Remove(info);
        Save();
    }

    private static string GetArchiveDir(ArchiveInfo info)
    {
        return Path.Combine(Folder, info.Id);
    }

    private static void Save()
    {
        Saves.SaveToSimpleScript(nameof(Saves), true, ArchivesDir);
    }
}
