using LocalUtilities.SimpleScript;
using LocalUtilities.SimpleScript.Common;
using LocalUtilities.SQLiteHelper;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Graph;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Policy;
using WarringStates.Flow;
using WarringStates.Map;
using WarringStates.User;

namespace WarringStates.Server.Map;

partial class AtlasEx
{
    public static string RootPath { get; } = Directory.CreateDirectory("saves").FullName;

    static SsSignTable SignTable { get; } = new();

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
        SerializeTool.SerializeFile(archiveInfo, new(), SignTable, true, GetArchiveInfoPath(archiveInfo.Id));
    }

    public static ArchiveInfo? LoadArchiveInfo(string archiveId)
    {
        return SerializeTool.DeserializeFile<ArchiveInfo>(new(), SignTable, GetArchiveInfoPath(archiveId));
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

    //public static bool CheckSourceSite(ArchiveInfo archiveInfo, Coordinate site)
    //{
    //    using var query = GetPlayerDatabaseQuery(archiveInfo);
    //    var sourceSite = new SourceSite() { Site = site };
    //    var condition = SQLiteQuery.GetCondition(sourceSite, Operators.Equal, nameof(SourceSite.Site));
    //    return query.SelectItems<SourceSite>(SOURCE_SITE, condition).Length is not 0;
    //}

    //public static bool CheckOwnerSite(ArchiveInfo archiveInfo, Coordinate site, [NotNullWhen(true)] out SourceSite? sourceSite, out string playerHashName)
    //{
    //    sourceSite = new SourceSite() { Site = site };
    //    var condition = SQLiteQuery.GetCondition(sourceSite, Operators.Equal, nameof(SourceSite.Site));
    //    using var query = GetOwnerSitesQuery(archiveInfo);
    //    query.Begin();
    //    var tableNames = query.ListAllTableNames();
    //    foreach (var table in tableNames)
    //    {
    //        sourceSite = query.SelectItems<SourceSite>(table, condition).FirstOrDefault();
    //        if (sourceSite is not null)
    //        {
    //            playerHashName = table;
    //            return true;
    //        }
    //    }
    //    playerHashName = string.Empty;
    //    return false;
    //}

    //public static List<OwnerSite> GetAllOwnerSites()
    //{
    //    using var query = CurrentArchiveInfo?.GetQuery();
    //    if (query is null)
    //        return [];
    //    return query.SelectItems<OwnerSite>(OWNER_SITES, null).ToList();
    //}

    //public static void RemoveOwnerSite(ArchiveInfo archiveInfo, Coordinate site)
    //{
    //    using var query = GetPlayerDatabaseQuery(archiveInfo);
    //    var sourceSite = new SourceSite() { Site = site };
    //    var condition = SQLiteQuery.GetCondition(sourceSite, Operators.Equal, nameof(SourceSite.Site));
    //    query.DeleteItems(SOURCE_SITE, condition);
    //}
}
