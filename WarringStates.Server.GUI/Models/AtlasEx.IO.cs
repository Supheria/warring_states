using LocalUtilities.SimpleScript;
using LocalUtilities.SQLiteHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Map;
using WarringStates.User;

namespace WarringStates.Server.GUI.Models;

partial class AtlasEx
{
    public static string GetFolderPath(string archiveId)
    {
        return Path.Combine(RootPath, archiveId);
    }

    public static string GetArchiveInfoPath(string archiveId)
    {
        return Path.Combine(GetFolderPath(archiveId), nameof(ArchiveInfo) + ".ss");
    }

    public static string GetLandPointsPath(string archiveId)
    {
        return Path.Combine(GetFolderPath(archiveId), nameof(TerrainSite) + ".ss");
    }

    public static void SaveArchiveInfo(ArchiveInfo archiveInfo)
    {
        SerializeTool.SerializeFile(archiveInfo, true, GetArchiveInfoPath(archiveInfo.Id));
    }

    public static ArchiveInfo? LoadArchiveInfo(string archiveId)
    {
        return SerializeTool.DeserializeFile<ArchiveInfo>(GetArchiveInfoPath(archiveId));
    }

    public static SQLiteQuery GetTerrainSiteDatabaseQuery(ArchiveInfo archiveInfo)
    {
        return new SQLiteQuery(Path.Combine(GetFolderPath(archiveInfo.Id), nameof(TerrainSite) + ".db"));
    }

    public static SQLiteQuery GetPlayerDatabaseQuery(ArchiveInfo archiveInfo)
    {
        return new SQLiteQuery(Path.Combine(GetFolderPath(archiveInfo.Id), nameof(Player) + ".db"));
    }

    public static void SaveTerrainSites(ArchiveInfo archiveInfo, List<TerrainSite> sites)
    {
        using var query = GetTerrainSiteDatabaseQuery(archiveInfo);
        query.Begin();
        var tableName = nameof(TerrainSite);
        query.CreateTable<TerrainSite>(tableName);
        query.InsertItems(tableName, sites.ToArray(), InsertTypes.ReplaceIfExists);
    }

    public static TerrainSite[] LoadLandPoints(ArchiveInfo archiveInfo)
    {
        using var query = GetTerrainSiteDatabaseQuery(archiveInfo);
        query.Begin();
        return query.SelectItems<TerrainSite>(nameof(TerrainSite), null);
    }

    public static long CurrentSpan
    {
        get => CurrentArchiveInfo?.CurrentSpan ?? 0;
        set
        {
            if (CurrentArchiveInfo is null)
                return;
            CurrentArchiveInfo.CurrentSpan = value;
            SaveArchiveInfo(CurrentArchiveInfo);
        }
    }
}
