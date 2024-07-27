using LocalUtilities.SimpleScript;
using LocalUtilities.SQLiteHelper;
using LocalUtilities.TypeGeneral;
using WarringStates.Map;
using WarringStates.Server.Map;
using WarringStates.User;

namespace WarringStates.Server.User;

partial class LocalArchive
{
    public static DataName NameofCurrentSpan { get; } = new(nameof(Archive.CurrentSpan));

    public static DataName NameofRandomTable { get; } = new(nameof(Archive.RandomTable));

    public static DataName NameofWorldSize { get; } = new(nameof(Archive.WorldSize));

    public static string NameofLandPoints { get; } = nameof(LandPoint);

    public static string NameofOwnerSites { get; } = nameof(OwnerSite);

    public static string GetArchiveRootPath(ArchiveInfo info)
    {
        return Path.Combine(RootPath, info.Id);
    }

    public static string GetCurrentSpanPath(ArchiveInfo info)
    {
        return Path.Combine(GetArchiveRootPath(info), "current span");
    }

    public static string GetRandomTablePath(ArchiveInfo info)
    {
        return Path.Combine(GetArchiveRootPath(info), "random table");
    }

    public static string GetWorldSizePath(ArchiveInfo info)
    {
        return Path.Combine(GetArchiveRootPath(info), "world size");
    }

    public static string GetThumbnailPath(ArchiveInfo info)
    {
        return Path.Combine(GetArchiveRootPath(info), "thumbnail");
    }

    private static SQLiteQuery GetArchiveQuery(ArchiveInfo info)
    {
        return new SQLiteQuery(Path.Combine(GetArchiveRootPath(info), "data.db"));
    }

    public static void SaveCurrentSpan(ArchiveInfo info, long currentSpan)
    {
        SerializeTool.SerializeFile(currentSpan, NameofCurrentSpan, SignTable, true, GetCurrentSpanPath(info));
    }

    public static long LoadCurrentSpan(ArchiveInfo info)
    {
        return SerializeTool.DeserializeFile<long>(NameofCurrentSpan, SignTable, GetCurrentSpanPath(info));
    }

    public static void SaveRandomTable(ArchiveInfo info, RandomTable randomTable)
    {
        SerializeTool.SerializeFile(randomTable, NameofRandomTable, SignTable, true, GetRandomTablePath(info));
    }

    public static RandomTable LoadRandomTable(ArchiveInfo info)
    {
        return SerializeTool.DeserializeFile<RandomTable>(NameofRandomTable, SignTable, GetRandomTablePath(info)) ?? new();
    }

    public static void SaveWorldSize(ArchiveInfo info, Size wordSize)
    {
        SerializeTool.SerializeFile(wordSize, NameofWorldSize, SignTable, true, GetWorldSizePath(info));
    }

    public static Size LoadWorldSize(ArchiveInfo info)
    {
        return SerializeTool.DeserializeFile<Size>(NameofWorldSize, SignTable, GetWorldSizePath(info));
    }

    public static void SaveLandPoints(ArchiveInfo info, List<LandPoint> landPoints)
    {
        using var query = GetArchiveQuery(info);
        query.CreateTable<LandPoint>(NameofLandPoints);
        query.InsertItems(NameofLandPoints, landPoints.ToArray());
    }

    public static List<LandPoint> LoadLandPoints(ArchiveInfo info)
    {
        using var query = GetArchiveQuery(info);
        return query.SelectItems<LandPoint>(NameofLandPoints, null).ToList();
    }

    public static void SaveThumbnail(ArchiveInfo info, LandMapEx landMap)
    {
        landMap.GetThumbnail().Save(GetThumbnailPath(info));
    }

    public static Bitmap LoadThumbnail(ArchiveInfo info)
    {
        using var stream = File.OpenRead(GetThumbnailPath(info));
        return (Bitmap)Image.FromStream(stream);
    }

    public static void InitOwnerSites(ArchiveInfo info)
    {
        using var query = GetArchiveQuery(info);
        query.CreateTable<OwnerSite>(NameofOwnerSites);
    }

    public static void SetOwnerSites(ArchiveInfo info, Coordinate site, SourceLandTypes landType, string playerId)
    {
        using var query = GetArchiveQuery(info);
        query.CreateTable<OwnerSite>(NameofOwnerSites);
        var ownerSite = new OwnerSite() { Site = site, LandType = landType, PlayerId = playerId };
        if (query.Exist(NameofOwnerSites, ownerSite))
            query.UpdateItem(NameofOwnerSites, ownerSite);
        else
            query.InsertItem(NameofOwnerSites, ownerSite);
    }

    public static List<OwnerSite> GetOwnerSites(ArchiveInfo info, string playerId)
    {
        using var query = GetArchiveQuery(info);
        var ownerSite = new OwnerSite() { PlayerId = playerId };
        var condition = SQLiteQuery.GetCondition(ownerSite, Operators.Equal, nameof(OwnerSite.PlayerId));
        return query.SelectItems<OwnerSite>(NameofOwnerSites, condition).ToList();
    }

    public static List<OwnerSite> GetOwnerSites(ArchiveInfo info)
    {
        using var query = GetArchiveQuery(info);
        return query.SelectItems<OwnerSite>(NameofOwnerSites, null).ToList();
    }

    public static void RemoveOwnerSite(ArchiveInfo info, Coordinate site)
    {
        using var query = GetArchiveQuery(info);
        var ownerSite = new OwnerSite() { Site = site };
        var condition = SQLiteQuery.GetCondition(ownerSite, Operators.Equal, nameof(OwnerSite.Site));
        query.DeleteItems(NameofOwnerSites, condition);
    }
}
