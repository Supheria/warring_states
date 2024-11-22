using LocalUtilities.SimpleScript;
using LocalUtilities.SimpleScript.Common;
using LocalUtilities.SQLiteHelper;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Graph;
using System.IO;
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
        return Path.Combine(GetFolderPath(archiveId), nameof(LandPoint) + ".ss");
    }

    public static void SaveArchiveInfo(ArchiveInfo archiveInfo)
    {
        SerializeTool.SerializeFile(archiveInfo, new(), SignTable, true, GetArchiveInfoPath(archiveInfo.Id));
    }

    public static ArchiveInfo? LoadArchiveInfo(string archiveId)
    {
        return SerializeTool.DeserializeFile<ArchiveInfo>(new(), SignTable, GetArchiveInfoPath(archiveId));
    }

    public static void SaveLandPoints(ArchiveInfo archiveInfo, List<LandPoint> landPoints)
    {
        SerializeTool.SerializeFile(landPoints, new(), SignTable, false, GetLandPointsPath(archiveInfo.Id));
    }

    public static List<LandPoint> LoadLandPoints(ArchiveInfo archiveInfo)
    {
        return SerializeTool.DeserializeFile<List<LandPoint>>(new(), SignTable, GetLandPointsPath(archiveInfo.Id)) ?? [];
    }

    public static SQLiteQuery GetOwnerSitesQuery(ArchiveInfo archiveInfo)
    {
        return new SQLiteQuery(Path.Combine(GetFolderPath(archiveInfo.Id), nameof(OwnerSite) + ".db"));
    }

    public static string GetPlayerTableName(Player player)
    {
        return player.GetNameHash();
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

    public static void SetOwnerSites(Coordinate site, SourceLandTypes landType, Player player)
    {
        if (CurrentArchiveInfo is null)
            return;
        using var query = GetOwnerSitesQuery(CurrentArchiveInfo);
        var ownerSite = new OwnerSite() { Site = site, LandType = landType };
        var tableName = GetPlayerTableName(player);
        if (query.Exist(tableName, ownerSite))
            query.UpdateItem(tableName, ownerSite);
        else
            query.InsertItem(tableName, ownerSite);
    }

    public static List<OwnerSite> GetOwnerSites(Player player)
    {
        if (CurrentArchiveInfo is null)
            return [];
        using var query = GetOwnerSitesQuery(CurrentArchiveInfo);
        return query.SelectItems<OwnerSite>(GetPlayerTableName(player), null).ToList();
    }

    //public static List<OwnerSite> GetAllOwnerSites()
    //{
    //    using var query = CurrentArchiveInfo?.GetQuery();
    //    if (query is null)
    //        return [];
    //    return query.SelectItems<OwnerSite>(OWNER_SITES, null).ToList();
    //}

    public static void RemoveOwnerSite(Player player, Coordinate site)
    {
        if (CurrentArchiveInfo is null)
            return;
        using var query = GetOwnerSitesQuery(CurrentArchiveInfo);
        var ownerSite = new OwnerSite() { Site = site };
        var condition = SQLiteQuery.GetCondition(ownerSite, Operators.Equal, nameof(OwnerSite.Site));
        query.DeleteItems(GetPlayerTableName(player), condition);
    }
}
