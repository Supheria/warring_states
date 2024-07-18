using AltitudeMapGenerator;
using LocalUtilities.SimpleScript;
using LocalUtilities.SimpleScript.Common;
using LocalUtilities.SQLiteHelper;
using System.Diagnostics.CodeAnalysis;

namespace WarringStates.Server.User;

internal static class LocalArchives
{
    public static string RootPath { get; } = Directory.CreateDirectory(nameof(Archives)).FullName;

    static string RegisterPath { get; } = Path.Combine(RootPath, nameof(Archives) + ".db");

    static List<ArchiveInfo> Archives { get; set; } = [];

    static DatabaseQuery Database { get; } = new();

    static string TableName { get; } = nameof(Archives);

    public static Archive? CurrentArchive { get; private set; } = null;

    //static int CurrentLoadIndex { get; set; } = -1;

    public static List<ArchiveInfo> ReLocate()
    {
        try
        {
            Archives.Clear();
            Database.Connect(RegisterPath);
            foreach (var fields in Database.SelectFieldsValue(TableName, [], TableTool.GetFieldsName<ArchiveInfo>()))
            {
                var info = new ArchiveInfo();
                TableTool.SetFieldsValue(info, fields);
                Archives.Add(info);
            }
        }
        catch { }
        finally
        {
            Database.Close();
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
            archive = info.Load();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool LoadArchive(int index/*, [NotNullWhen(true)] out Archive? archive*/)
    {
        //archive = null;
        if (!TryGetArchiveInfo(index, out var info))
            return false;
        try
        {
            CurrentArchive = info.Load();
            UserException.ThrowIfNotUseable(info);
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
        var info = new ArchiveInfo(worldName, mapData.Size);
        Directory.CreateDirectory(info.RootPath);
        var archive = info.Create(mapData);
        info.Save(archive);
        var saves = new List<ArchiveInfo>() { info };
        saves.AddRange(Archives);
        Archives = saves;
        //Save();
        var fields = TableTool.GetFieldsValue(info);
        Database.Connect(RegisterPath);
        Database.CreateTable(TableName, fields);
        Database.InsertFieldsValue(TableName, fields);
        Database.Close();
        return info;
    }

    public static void Update(int index)
    {
        if (!TryGetArchiveInfo(index, out var info) || CurrentArchive is null)
            return;
        info.Save(CurrentArchive);
        Archives.Remove(info);
        info.UpdateLastSaveTime();
        var saves = new List<ArchiveInfo>() { info };
        saves.AddRange(Archives);
        Archives = saves;
    }

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
        try
        {
            Database.Connect(RegisterPath);
            var fields = TableTool.GetFieldsName<ArchiveInfo>();
            Database.CreateTable(TableName, fields);
            foreach (var info in Archives)
            {
                fields = TableTool.GetFieldsValue(info);
                Database.UpdateFieldsValues(TableName, [], fields);
            }
        }
        catch { }
        finally
        {
            Database.Close();
        }
    }
}
