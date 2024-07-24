using AltitudeMapGenerator;
using LocalUtilities.SimpleScript;
using LocalUtilities.SimpleScript.Common;
using LocalUtilities.SQLiteHelper;
using LocalUtilities.SQLiteHelper.Data;
using LocalUtilities.TypeGeneral;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using WarringStates.Map;
using WarringStates.Server.Events;
using WarringStates.Server.Map;
using WarringStates.User;

namespace WarringStates.Server.User;

internal class LocalArchives
{
    public static string RootPath { get; } = Directory.CreateDirectory(nameof(Archive)).FullName;

    static string RegisterPath { get; } = Path.Combine(RootPath, nameof(ArchiveInfoList) + ".db");

    public static ArchiveInfoList ArchiveInfoList { get; } = [];

    static string TableName { get; } = nameof(ArchiveInfoList);

    public static Archive? CurrentArchive { get; private set; } = null;

    static SsSignTable SignTable { get; } = new();

    public static void ReLocate()
    {
        try
        {
            var players = new Players();
            for (var i = 0; i < 10000; i++)
            {
                var player = new Player("test" + i, "13324");
                players.TryAdd(player);
            }
            SerializeTool.SerializeFile(players, new("Players"), SignTable, true, "players");
            var a = SerializeTool.DeserializeFile<Players>(new("Players"), SignTable, "players").Count;
            var stop = new Stopwatch();
            stop.Start();
            using var query = new SQLiteQuery("test.db");
            var field = TableTool.GetFieldsName<Player>();
            var x = query.Sum("test", null, null);
            stop.Stop();
            query.CreateTable("test", field);
            foreach(var p in players)
            {
                field = TableTool.GetFieldsValue(p);
                query.InsertFieldsValue("test", field);
            }
            query.Dispose();
            stop.Stop();
            //using var query = new SQLiteQuery(RegisterPath);
            ArchiveInfoList.Clear();
            TableTool.GetFieldName<ArchiveInfo>(nameof(ArchiveInfo.WorldName), out var f);
            var count = query.Sum(TableName, f, null);
            foreach (var fields in query.SelectFieldsValue(TableName, TableTool.GetFieldsName<ArchiveInfo>(), null))
            {
                var info = new ArchiveInfo();
                TableTool.SetFieldsValue(info, fields);
                ArchiveInfoList.TryAdd(info);
            }
            BroadCastUpdate();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private  static void BroadCastUpdate()
    {
        LocalEvents.TryBroadcast(LocalEvents.UserInterface.ArchiveListRefreshed);
    }

    public static bool LoadArchive(int intdex)
    {
        try
        {
            if (!ArchiveInfoList.TryGetValue(intdex, out var info))
                return false;
            CurrentArchive = LoadArchive(info);
            //UserException.ThrowIfNotUseable(info);
            LocalEvents.TryBroadcast(LocalEvents.UserInterface.ArchiveToLoad);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            return false;
        }
    }

    public static bool CreateArchive(AltitudeMapData mapData, string worldName, IProgressor progressor)
    {
        try
        {
            var info = new ArchiveInfo(worldName, mapData.Size);
            Directory.CreateDirectory(GetArchiveRootPath(info));
            var archive = CreateArchive(info, mapData, progressor);
            SaveArchive(info, archive);
            var fields = TableTool.GetFieldsValue(info);
            using var query = new SQLiteQuery(RegisterPath);
            query.CreateTable(TableName, fields);
            query.InsertFieldsValue(TableName, fields);
            ArchiveInfoList.TryAdd(info);
            BroadCastUpdate();
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            return false;
        }
    }

    public static void Update(int index)
    {
        try
        {
            if (!ArchiveInfoList.TryGetValue(index, out var info) || CurrentArchive is null)
                return;
            SaveArchive(info, CurrentArchive);
            info.UpdateLastSaveTime();
            using var query = new SQLiteQuery(RegisterPath);
            if (!TableTool.GetFieldName<ArchiveInfo>(nameof(ArchiveInfo.Id), out var field))
                return;
            var condition = new Condition(field.Name, info.Id, Condition.Operates.Equal);
            query.UpdateFieldsValues(TableName, TableTool.GetFieldsValue(info), condition);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

    }

    public static bool Delete(int index)
    {
        try
        {
            if (!ArchiveInfoList.TryGetValue(index, out var info))
                return false;
            if (MessageBox.Show($"要永远删除 {info.WorldName} 吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                return false;
            Directory.Delete(GetArchiveRootPath(info), true);
            using var query = new SQLiteQuery(RegisterPath);
            if (!TableTool.GetFieldName<ArchiveInfo>(nameof(ArchiveInfo.Id), out var field))
                return false;
            var condition = new Condition(field.Name, info.Id, Condition.Operates.Equal);
            query.DeleteFields(TableName, condition);
            ArchiveInfoList.TryRemove(info);
            BroadCastUpdate();
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            return false;
        }
    }

    public static string GetArchiveRootPath(ArchiveInfo info)
    {
        return Path.Combine(RootPath, info.Id);
    }

    public static string GetThumbnailPath(ArchiveInfo info)
    {
        return Path.Combine(GetArchiveRootPath(info), "thumbnail");
    }

    public static string GetAltitudeMapPath(ArchiveInfo info)
    {
        return Path.Combine(GetArchiveRootPath(info), "altitude map");
    }

    public static string GetRandomTablePath(ArchiveInfo info)
    {
        return Path.Combine(GetArchiveRootPath(info), "random table");
    }

    public static string GetSourceLandsPath(ArchiveInfo info)
    {
        return Path.Combine(GetArchiveRootPath(info), "source lands");
    }

    public static string GetPlayersPath(ArchiveInfo info)
    {
        return Path.Combine(GetArchiveRootPath(info), "players");
    }

    public static string GetCurrentSpanPath(ArchiveInfo info)
    {
        return Path.Combine(GetArchiveRootPath(info), "current span");
    }

    public static bool LoadThumbnail(ArchiveInfo info, [NotNullWhen(true)] out Bitmap? thumbnail)
    {
        try
        {
            using var stream = File.OpenRead(GetThumbnailPath(info));
            thumbnail = new(stream);
            return true;
        }
        catch
        {
            thumbnail = null;
            return false;
        }
    }

    public static AltitudeMap LoadAltitudeMap(ArchiveInfo info)
    {
        try
        {
            return SerializeTool.DeserializeFile<AltitudeMap>(new(nameof(Archive.AltitudeMap)), SignTable, GetAltitudeMapPath(info)) ?? new();
        }
        catch
        {
            return new();
        }
    }

    public static RandomTable LoadRandomTable(ArchiveInfo info)
    {
        try
        {
            return SerializeTool.DeserializeFile<RandomTable>(new(nameof(Archive.RandomTable)), SignTable, GetRandomTablePath(info)) ?? new();
        }
        catch
        {
            return new();
        }
    }

    public static SourceLands LoadSourceLands(ArchiveInfo info)
    {
        try
        {
            return SerializeTool.DeserializeFile<SourceLands>(new(nameof(Archive.SourceLands)), SignTable, GetSourceLandsPath(info)) ?? new();
        }
        catch
        {
            return new();
        }
    }

    public static Players LoadPlayers(ArchiveInfo info)
    {
        try
        {
            return SerializeTool.DeserializeFile<Players>(new(nameof(Archive.Players)), SignTable, GetPlayersPath(info)) ?? [];
        }
        catch
        {
            return [];
        }
    }

    public static long LoadCurrentSpan(ArchiveInfo info)
    {
        try
        {
            return SerializeTool.DeserializeFile<long>(new(nameof(Archive.CurrentSpan)), SignTable, GetCurrentSpanPath(info));
        }
        catch
        {
            return 0;
        }
    }

    public static Archive LoadArchive(ArchiveInfo info)
    {
        return new()
        {
            AltitudeMap = LoadAltitudeMap(info),
            RandomTable = LoadRandomTable(info),
            SourceLands = LoadSourceLands(info),
            Players = LoadPlayers(info),
            CurrentSpan = LoadCurrentSpan(info),
        };
    }

    public static void SaveArchive(ArchiveInfo info, Archive archive)
    {
        SerializeTool.SerializeFile(archive.AltitudeMap, new(nameof(Archive.AltitudeMap)), SignTable, false, GetAltitudeMapPath(info));
        SerializeTool.SerializeFile(archive.RandomTable, new(nameof(Archive.RandomTable)), SignTable, false, GetRandomTablePath(info));
        SerializeTool.SerializeFile(archive.SourceLands, new(nameof(Archive.SourceLands)), SignTable, true, GetSourceLandsPath(info));
        SerializeTool.SerializeFile(archive.Players, new(nameof(Archive.Players)), SignTable, true, GetPlayersPath(info));
        SerializeTool.SerializeFile(archive.CurrentSpan, new(nameof(Archive.CurrentSpan)), SignTable, true, GetCurrentSpanPath(info));
    }

    public static Archive CreateArchive(ArchiveInfo info, AltitudeMapData mapData, IProgressor progressor)
    {
        Directory.CreateDirectory(Path.Combine(RootPath, info.Id));
        var altitudeMap = new AltitudeMap(mapData, progressor);
        var landMap = new LandMap();
        var randomTable = new RandomTable(1000);
        landMap.Relocate(altitudeMap, randomTable);
        using var thumbnail = new Bitmap(landMap.Width, landMap.Height);
        var pThumbnail = new PointBitmap(thumbnail);
        pThumbnail.LockBits();
        for (int i = 0; i < landMap.Width; i++)
        {
            for (int j = 0; j < landMap.Height; j++)
            {
                var color = landMap[new(i, j)].Color;
                pThumbnail.SetPixel(i, j, color);
            }
        }
        pThumbnail.UnlockBits();
        thumbnail.Save(GetThumbnailPath(info));
        return new(altitudeMap, randomTable);
    }
}
