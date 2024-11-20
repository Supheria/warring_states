using LocalUtilities.SQLiteHelper;
using LocalUtilities.TypeGeneral;
using WarringStates.Map;

namespace WarringStates.Server.Map;

partial class AtlasEx
{
    public const string OWNER_SITES = "owner sites";

    public static long GetCurrentSpan()
    {
        return CurrentArchiveInfo?.CurrentSpan ?? 0;
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
