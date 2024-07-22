using AltitudeMapGenerator;
using LocalUtilities.SimpleScript;
using LocalUtilities.SimpleScript.Common;
using LocalUtilities.SQLiteHelper;
using LocalUtilities.SQLiteHelper.Data;
using LocalUtilities.TypeGeneral;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using WarringStates.Map.Terrain;
using WarringStates.Map;
using WarringStates.Server.Events;
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
            ArchiveInfoList.Clear();
            using var query = new SQLiteQuery(RegisterPath);
            foreach (var fields in query.SelectFieldsValue(TableName, TableTool.GetFieldsName<ArchiveInfo>(), null)) 
            {
                var info = new ArchiveInfo();
                TableTool.SetFieldsValue(info, fields);
                ArchiveInfoList.Add(info);
            }
            LocalEvents.TryBroadcast(LocalEvents.UserInterface.ArchiveListRefreshed);
        }
        catch(Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
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

    public static bool CreateArchive(AltitudeMapData mapData, string worldName)
    {
        try
        {
            var info = new ArchiveInfo(worldName, mapData.Size);
            Directory.CreateDirectory(GetArchiveRootPath(info));
            var archive = CreateArchive(info, mapData);
            SaveArchive(info, archive);
            var fields = TableTool.GetFieldsValue(info);
            using var query = new SQLiteQuery(RegisterPath);
            query.CreateTable(TableName, fields);
            query.InsertFieldsValue(TableName, fields);
            ReLocate();
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
            ReLocate() ;
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
            ReLocate();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string GetArchiveRootPath(ArchiveInfo info)
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

    public static Bitmap LoadThumbnail(ArchiveInfo info)
    {
        return (Bitmap)Image.FromFile(GetThumbnailPath(info));
    }

    public static AltitudeMap LoadAltitudeMap(ArchiveInfo info)
    {
        return SerializeTool.DeserializeFile<AltitudeMap>(new(nameof(Archive.AltitudeMap)), GetAltitudeMapPath(info), SignTable) ?? new();
    }

    public static RandomTable LoadRandomTable(ArchiveInfo info)
    {
        return SerializeTool.DeserializeFile<RandomTable>(new(nameof(Archive.RandomTable)), GetRandomTablePath(info), SignTable) ?? new();
    }

    public static Dictionary<string, List<SourceLand>> LoadSourceLands(ArchiveInfo info)
    {
        return SerializeTool.DeserializeFile<Dictionary<string, List<SourceLand>>>(new(nameof(Archive.SourceLands)), GetSourceLandsPath(info), SignTable) ?? [];
    }

    public static Players LoadPlayers(ArchiveInfo info)
    {
        return SerializeTool.DeserializeFile<Players>(new(nameof(Archive.Players)), GetPlayersPath(info), SignTable) ?? [];
    }

    public static int LoadCurrentSpan(ArchiveInfo info)
    {
        return SerializeTool.DeserializeFile<int>(new(nameof(Archive.CurrentSpan)), GetCurrentSpanPath(info), SignTable);
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
        SerializeTool.SerializeFile(archive.AltitudeMap, new(nameof(Archive.AltitudeMap)), GetAltitudeMapPath(info), false, SignTable);
        SerializeTool.SerializeFile(archive.RandomTable, new(nameof(Archive.RandomTable)), GetRandomTablePath(info), false, SignTable);
        SerializeTool.SerializeFile(archive.SourceLands, new(nameof(Archive.SourceLands)), GetSourceLandsPath(info), true, SignTable);
        SerializeTool.SerializeFile(archive.Players, new(nameof(Archive.Players)), GetPlayersPath(info), true, SignTable);
        SerializeTool.SerializeFile(archive.CurrentSpan, new(nameof(Archive.CurrentSpan)), GetCurrentSpanPath(info), true, SignTable);
    }

    public static Archive CreateArchive(ArchiveInfo info, AltitudeMapData mapData)
    {
        Directory.CreateDirectory(Path.Combine(RootPath, info.Id));
        var altitudeMap = new AltitudeMap(mapData);
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
        return new(/*info, */altitudeMap, randomTable);
    }
}
