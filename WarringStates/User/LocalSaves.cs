using AltitudeMapGenerator;
using LocalUtilities.SimpleScript.Serialization;
using System.Diagnostics.CodeAnalysis;

namespace WarringStates.User;

public static class LocalSaves
{
    static string RegisterPath => ArchiveInfo.RegisterPath;

    static List<ArchiveInfo> Saves { get; set; } = [];
    //[
    //new("测试存档中文1"),
    //];
    //[];
    //[
    //new ("测试存档中文1"),
    //new ("测试存档中文20A"),
    //new ("测试存档中文300B"),
    //new ("测试存档中文4"),
    //new ("测试存档中文5"),
    //new ("测试存档中文6"),
    //new ("测试存档中文7"),
    //new ("测试存档中文8"),
    //new ("测试存档中文9"),
    //];

    public static List<ArchiveInfo> ReLocate()
    {
        try
        {
            Saves = SerializeTool.LoadFromSimpleScript<ArchiveInfo>(nameof(Saves), RegisterPath).ToList();
        }
        catch
        {
            Save();
        }
        return Saves;
    }

    public static int Count => Saves.Count;

    public static bool TryGetArchiveInfo(int index, [NotNullWhen(true)] out ArchiveInfo? info)
    {
        info = null;
        if (index < 0 || index >= Count)
            return false;
        info = Saves[index];
        return true;
    }

    public static bool TryGetArchive(int index, [NotNullWhen(true)] out Archive? archive)
    {
        archive = null;
        if (!TryGetArchiveInfo(index, out var info))
            return false;
        try
        {
            archive = new Archive().LoadFromSimpleScript(info.GetArchivePath());
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool LoadArchive(int index, [NotNullWhen(true)] out Archive? archive)
    {
        archive = null;
        if (!TryGetArchiveInfo(index, out var info))
            return false;
        var dir = info.GetArchivePath();
        try
        {
            archive = new Archive().LoadFromSimpleScript(dir);
            UserException.ThrowIfNotUseable(archive);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{ex.Message}", $"无法加载 - {info.WorldName}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }

    public static ArchiveInfo CreateArchive(this AltitudeMapData mapData, string worldName)
    {
        var info = new ArchiveInfo(worldName);
        Archive.Create(info, mapData).SaveToSimpleScript(false, info.GetArchivePath());
        var saves = new List<ArchiveInfo>() { info };
        saves.AddRange(Saves);
        Saves = saves;
        Save();
        return info;
    }

    public static void Update(this Archive archive)
    {
        Saves.Remove(archive.Info);
        archive.Info.UpdateLastSaveTime();
        archive.SaveToSimpleScript(false, archive.Info.GetArchivePath());
        var saves = new List<ArchiveInfo>() { archive.Info };
        saves.AddRange(Saves);
        Saves = saves;
    }

#if DEBUG
    public static void Update(int index)
    {
        if (!TryGetArchiveInfo(index, out var info))
            return;
        Saves.Remove(info);
        info.UpdateLastSaveTime();
        var saves = new List<ArchiveInfo>() { info };
        saves.AddRange(Saves);
        Saves = saves;
    }
#endif

    public static bool Delete(int index)
    {
        if (!TryGetArchiveInfo(index, out var info))
            return false;
        if (MessageBox.Show($"要永远删除 {info.WorldName} 吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
            return false;
        var path = info.GetArchivePath();
        if (File.Exists(path))
            File.Delete(path);
        path = info.GetThumbnailPath();
        if (File.Exists(path))
            File.Delete(path);
        Saves.Remove(info);
        Save();
        return true;
    }

    private static void Save()
    {
        Saves.SaveToSimpleScript(nameof(Saves), true, RegisterPath);
    }
}
