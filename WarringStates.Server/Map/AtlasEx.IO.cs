using LocalUtilities.SimpleScript;
using LocalUtilities.SimpleScript.Common;
using LocalUtilities.SQLiteHelper;
using LocalUtilities.TypeGeneral;
using System.IO;
using WarringStates.Flow;
using WarringStates.Map;
using WarringStates.User;

namespace WarringStates.Server.Map;

partial class AtlasEx
{
    public const string ARCHIVE_INFO = "archive_info.ss";
    public const string OWNER_SITES = "owner_sites.ss";

    public static string RootPath { get; } = Directory.CreateDirectory("saves").FullName;

    static SsSignTable SignTable { get; } = new();

    public static string GetFolderPath(string archiveId)
    {
        return Path.Combine(RootPath, archiveId);
    }

    public static string GetArchiveInfoPath(string archiveId)
    {
        return Path.Combine(GetFolderPath(archiveId), ARCHIVE_INFO);
    }

    public static string GetLandPointsPath(string archiveId)
    {
        return Path.Combine(GetFolderPath(archiveId), OWNER_SITES);
    }

    public static void SaveArchiveInfo(ArchiveInfo archiveInfo)
    {
        SerializeTool.SerializeFile(archiveInfo, new(), SignTable, true, GetArchiveInfoPath(archiveInfo.Id));
    }

    public static ArchiveInfo? LoadArchiveInfo(string archiveId)
    {
        return SerializeTool.DeserializeFile<ArchiveInfo>(new(), SignTable, GetArchiveInfoPath(archiveId));
    }

    public static void SaveLandPoints(ArchiveInfo archiveInfo, List<LandPoint>landPoints)
    {
        SerializeTool.SerializeFile(landPoints, new(), SignTable, false, GetLandPointsPath(archiveInfo.Id));
    }

    public static List<LandPoint> LoadLandPoints(ArchiveInfo archiveInfo)
    {
        return SerializeTool.DeserializeFile<List<LandPoint>>(new(), SignTable, GetLandPointsPath(archiveInfo.Id)) ?? [];
    }

    public static SQLiteQuery GetPlayerDatabaseQuery(ArchiveInfo archiveInfo, Player player)
    {
        return new SQLiteQuery(Path.Combine(GetFolderPath(archiveInfo.Id), player.GetHashId() + ".db"));
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
        using var query = GetPlayerDatabaseQuery(CurrentArchiveInfo, player);
        query.CreateTable<OwnerSite>(OWNER_SITES);
        var ownerSite = new OwnerSite() { Site = site, LandType = landType };
        if (query.Exist(OWNER_SITES, ownerSite))
            query.UpdateItem(OWNER_SITES, ownerSite);
        else
            query.InsertItem(OWNER_SITES, ownerSite);
    }

    public static List<OwnerSite> GetOwnerSites(Player player)
    {
        if (CurrentArchiveInfo is null)
            return [];
        using var query = GetPlayerDatabaseQuery(CurrentArchiveInfo, player);
        return query.SelectItems<OwnerSite>(OWNER_SITES, null).ToList();
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
        using var query = GetPlayerDatabaseQuery(CurrentArchiveInfo, player);
        var ownerSite = new OwnerSite() { Site = site };
        var condition = SQLiteQuery.GetCondition(ownerSite, Operators.Equal, nameof(OwnerSite.Site));
        query.DeleteItems(OWNER_SITES, condition);
    }
}
