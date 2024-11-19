using LocalUtilities.SimpleScript;
using LocalUtilities.SimpleScript.Common;
using LocalUtilities.SQLiteHelper;
using LocalUtilities.TypeGeneral;
using WarringStates.Map;

namespace WarringStates.Server.Map;

partial class AtlasEx
{
    public const string CURRENT_SPAN = "current span";
    public const string RADOM_TABLE = "random table";
    public const string WORLD_SIZE = "world size";
    public const string LAND_POINTS = "land points";
    public const string OWNER_SITES = "owner sites";
    public const string THUMBNAIL = "thumbnail";

    static SsSignTable SignTable { get; } = new();

    //public static bool GetArchiveRootPath(out string path)
    //{
    //    path = string.Empty;
    //    if (CurrentArchiveInfo is null)
    //        return false;
    //    path = Path.Combine(RootPath, CurrentArchiveInfo.Id);
    //    return true;
    //}

    //public static bool GetCurrentSpanPath(out string path)
    //{
    //    path = string.Empty;
    //    if (!GetArchiveRootPath(out var root))
    //        return false;
    //    path = Path.Combine(root, CURRENT_SPAN);
    //    return true;
    //}

    //public static bool GetRandomTablePath(out string path)
    //{
    //    path = string.Empty;
    //    if (!GetArchiveRootPath(out var root))
    //        return false;
    //    path = Path.Combine(root, RADOM_TABLE);
    //    return true;
    //}

    //public static bool GetWorldSizePath(out string path)
    //{
    //    path = string.Empty;
    //    if (!GetArchiveRootPath(out var root))
    //        return false;
    //    path = Path.Combine(root, WORLD_SIZE);
    //    return true;
    //}

    //public static bool GetThumbnailPath(out string path)
    //{
    //    path = string.Empty;
    //    if (!GetArchiveRootPath(out var root))
    //        return false;
    //    path = Path.Combine(root, THUMBNAIL);
    //    return true;
    //}

    public static void SaveCurrentSpan(long currentSpan)
    {
        CurrentArchiveInfo?.UpdateCurrentSpan(currentSpan);
    }

    public static long LoadCurrentSpan()
    {
        return CurrentArchiveInfo?.CurrentSpan ?? 0;
    }

    public static List<LandPoint> LoadLandPoints()
    {
        using var query = CurrentArchiveInfo?.GetQuery();
        if (query is null)
            return [];
        return query.SelectItems<LandPoint>(LAND_POINTS, null).ToList() ?? [];
    }

    public static void SetOwnerSites(Coordinate site, SourceLandTypes landType, string playerName)
    {
        using var query = CurrentArchiveInfo?.GetQuery();
        if (query is null)
            return;
        query.CreateTable<OwnerSite>(OWNER_SITES);
        var ownerSite = new OwnerSite() { Site = site, LandType = landType, PlayerName = playerName };
        if (query.Exist(OWNER_SITES, ownerSite))
            query.UpdateItem(OWNER_SITES, ownerSite);
        else
            query.InsertItem(OWNER_SITES, ownerSite);
    }

    public static List<OwnerSite> GetOwnerSites(string playerName)
    {
        using var query = CurrentArchiveInfo?.GetQuery();
        if (query is null)
            return [];
        var ownerSite = new OwnerSite() { PlayerName = playerName };
        var condition = SQLiteQuery.GetCondition(ownerSite, Operators.Equal, nameof(OwnerSite.PlayerName));
        return query.SelectItems<OwnerSite>(OWNER_SITES, condition).ToList();
    }

    public static List<OwnerSite> GetOwnerSites()
    {
        using var query = CurrentArchiveInfo?.GetQuery();
        if (query is null)
            return [];
        return query.SelectItems<OwnerSite>(OWNER_SITES, null).ToList();
    }

    public static void RemoveOwnerSite(Coordinate site)
    {
        using var query = CurrentArchiveInfo?.GetQuery();
        if (query is null)
            return;
        var ownerSite = new OwnerSite() { Site = site };
        var condition = SQLiteQuery.GetCondition(ownerSite, Operators.Equal, nameof(OwnerSite.Site));
        query.DeleteItems(OWNER_SITES, condition);
    }
}
