using LocalUtilities.SimpleScript.Common;
using LocalUtilities.SimpleScript;
using LocalUtilities.SQLiteHelper;
using LocalUtilities.TypeGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Map;

namespace WarringStates.Server.Map;

partial class Atlas
{
    public const string CURRENT_SPAN = "current span";
    public const string RADOM_TABLE = "random table";
    public const string WORLD_SIZE = "world size";
    public const string LAND_POINTS = "land points";
    public const string OWNER_SITES = "owner sites";
    public const string THUMBNAIL = "thumbnail";
    public const string DATABASE = "data.base";

    static SsSignTable SignTable { get; } = new();

    public static string GetArchiveRootPath()
    {
        if (CurrentArchiveInfo is null)
            throw new ArgumentNullException(nameof(CurrentArchiveInfo));
        return Path.Combine(RootPath, CurrentArchiveInfo.Id);
    }

    public static string GetCurrentSpanPath()
    {
        return Path.Combine(GetArchiveRootPath(), CURRENT_SPAN);
    }

    public static string GetRandomTablePath()
    {
        return Path.Combine(GetArchiveRootPath(), RADOM_TABLE);
    }

    public static string GetWorldSizePath()
    {
        return Path.Combine(GetArchiveRootPath(), WORLD_SIZE);
    }

    public static string GetThumbnailPath()
    {
        return Path.Combine(GetArchiveRootPath(), THUMBNAIL);
    }

    private static SQLiteQuery GetArchiveQuery()
    {
        return new SQLiteQuery(Path.Combine(GetArchiveRootPath(), DATABASE));
    }

    public static void SaveCurrentSpan(long currentSpan)
    {
        SerializeTool.SerializeFile(currentSpan, new(CURRENT_SPAN), SignTable, true, GetCurrentSpanPath());
    }

    public static long LoadCurrentSpan()
    {
        return SerializeTool.DeserializeFile<long>(new(CURRENT_SPAN), SignTable, GetCurrentSpanPath());
    }

    public static void SaveRandomTable(RandomTable randomTable)
    {
        SerializeTool.SerializeFile(randomTable, new(RADOM_TABLE), SignTable, true, GetRandomTablePath());
    }

    public static RandomTable LoadRandomTable()
    {
        return SerializeTool.DeserializeFile<RandomTable>(new(RADOM_TABLE), SignTable, GetRandomTablePath()) ?? new();
    }

    public static void SaveWorldSize(Size wordSize)
    {
        SerializeTool.SerializeFile(wordSize, new(WORLD_SIZE), SignTable, true, GetWorldSizePath());
    }

    public static Size LoadWorldSize()
    {
        return SerializeTool.DeserializeFile<Size>(new(WORLD_SIZE), SignTable, GetWorldSizePath());
    }

    public static void SaveLandPoints(List<LandPoint> landPoints)
    {
        using var query = GetArchiveQuery();
        query.CreateTable<LandPoint>(LAND_POINTS);
        query.InsertItems(LAND_POINTS, landPoints.ToArray());
    }

    public static List<LandPoint> LoadLandPoints()
    {
        using var query = GetArchiveQuery();
        return query.SelectItems<LandPoint>(LAND_POINTS, null).ToList();
    }

    public static void SaveThumbnail(Bitmap thumbnail)
    {
        thumbnail.Save(GetThumbnailPath());
    }

    public static Bitmap LoadThumbnail()
    {
        using var stream = File.OpenRead(GetThumbnailPath());
        return (Bitmap)Image.FromStream(stream);
    }

    public static void InitOwnerSites()
    {
        using var query = GetArchiveQuery();
        query.CreateTable<OwnerSite>(OWNER_SITES);
    }

    public static void SetOwnerSites(Coordinate site, SourceLandTypes landType, string playerName)
    {
        using var query = GetArchiveQuery();
        query.CreateTable<OwnerSite>(OWNER_SITES);
        var ownerSite = new OwnerSite() { Site = site, LandType = landType, PlayerName = playerName };
        if (query.Exist(OWNER_SITES, ownerSite))
            query.UpdateItem(OWNER_SITES, ownerSite);
        else
            query.InsertItem(OWNER_SITES, ownerSite);
    }

    public static List<OwnerSite> GetOwnerSites(string playerName)
    {
        using var query = GetArchiveQuery();
        var ownerSite = new OwnerSite() { PlayerName = playerName };
        var condition = SQLiteQuery.GetCondition(ownerSite, Operators.Equal, nameof(OwnerSite.PlayerName));
        return query.SelectItems<OwnerSite>(OWNER_SITES, condition).ToList();
    }

    public static List<OwnerSite> GetOwnerSites()
    {
        using var query = GetArchiveQuery();
        return query.SelectItems<OwnerSite>(OWNER_SITES, null).ToList();
    }

    public static void RemoveOwnerSite(Coordinate site)
    {
        using var query = GetArchiveQuery();
        var ownerSite = new OwnerSite() { Site = site };
        var condition = SQLiteQuery.GetCondition(ownerSite, Operators.Equal, nameof(OwnerSite.Site));
        query.DeleteItems(OWNER_SITES, condition);
    }
}
