using AltitudeMapGenerator;
using LocalUtilities.SimpleScript.Serialization;
using System.Diagnostics.CodeAnalysis;

namespace WarringStates.Server.User;

public static class LocalArchives
{
    public static string RootPath { get; } = Directory.CreateDirectory(nameof(Archives)).FullName;

    static string RegisterPath { get; } = Path.Combine(RootPath, nameof(Archives));

    static List<ArchiveInfo> Archives { get; set; } = [];
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
            Archives = SerializeTool.LoadFromSimpleScript<ArchiveInfo>(nameof(Archives), RegisterPath).ToList();
        }
        catch
        {
            Save();
        }
        return Archives;
    }

    public static int Count => Archives.Count;

    public static bool TryGetArchiveInfo(int index, [NotNullWhen(true)] out ArchiveInfo? info)
    {
        info = null;
        if (index < 0 || index >= Count)
            return false;
        info = Archives[index];
        return true;
    }

    public static bool TryGetArchive(int index, [NotNullWhen(true)] out Archive? archive)
    {
        archive = null;
        if (!TryGetArchiveInfo(index, out var info))
            return false;
        try
        {
            archive = Archive.Load(info);
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
        try
        {
            archive = Archive.Load(info);
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
        Archive.Create(info, mapData).Save();
        var saves = new List<ArchiveInfo>() { info };
        saves.AddRange(Archives);
        Archives = saves;
        Save();
        return info;
    }

    public static void Update(this Archive archive)
    {
        Archives.Remove(archive.Info);
        archive.Save();
        var saves = new List<ArchiveInfo>() { archive.Info };
        saves.AddRange(Archives);
        Archives = saves;
    }

#if DEBUG
    public static void Update(int index)
    {
        if (!TryGetArchiveInfo(index, out var info))
            return;
        Archives.Remove(info);
        info.UpdateLastSaveTime();
        var saves = new List<ArchiveInfo>() { info };
        saves.AddRange(Archives);
        Archives = saves;
    }
#endif

    public static bool Delete(int index)
    {
        if (!TryGetArchiveInfo(index, out var info))
            return false;
        if (MessageBox.Show($"要永远删除 {info.WorldName} 吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
            return false;
        var path = info.RootPath;
        try
        {
            Directory.Delete(path, true);
        }
        catch { }
        Archives.Remove(info);
        Save();
        return true;
    }

    private static void Save()
    {
        Archives.SaveToSimpleScript(nameof(Archives), true, RegisterPath);
    }
}
