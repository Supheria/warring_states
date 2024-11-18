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
using System.IO;
using System.Diagnostics.CodeAnalysis;

namespace WarringStates.Server.Map;

partial class AtlasEx
{
    public const string CURRENT_SPAN = "current span";
    public const string RADOM_TABLE = "random table";
    public const string WORLD_SIZE = "world size";
    public const string LAND_POINTS = "land points";
    public const string OWNER_SITES = "owner sites";
    public const string THUMBNAIL = "thumbnail";
    public const string DATABASE = "data.base";

    static SsSignTable SignTable { get; } = new();

    public static bool GetArchiveRootPath(out string path)
    {
        path = string.Empty;
        if (CurrentArchiveInfo is null)
            return false;
        path = Path.Combine(RootPath, CurrentArchiveInfo.Id);
        return true;
    }

    public static bool GetCurrentSpanPath(out string path)
    {
        path = string.Empty;
        if (!GetArchiveRootPath(out var root))
            return false;
        path = Path.Combine(root, CURRENT_SPAN);
        return true;
    }

    public static bool GetRandomTablePath(out string path)
    {
        path = string.Empty;
        if (!GetArchiveRootPath(out var root))
            return false;
        path = Path.Combine(root, RADOM_TABLE);
        return true;
    }

    public static bool GetWorldSizePath(out string path)
    {
        path = string.Empty;
        if (!GetArchiveRootPath(out var root))
            return false;
        path = Path.Combine(root, WORLD_SIZE);
        return true;
    }

    public static bool GetThumbnailPath(out string path)
    {
        path = string.Empty;
        if (!GetArchiveRootPath(out var root))
            return false;
        path = Path.Combine(root, THUMBNAIL);
        return true;
    }

    private static SQLiteQuery? GetArchiveQuery()
    {
        if (GetArchiveRootPath(out var root))
            return new SQLiteQuery(Path.Combine(root, DATABASE));
        return null;
    }

    public static void SaveCurrentSpan(long currentSpan)
    {
        if (GetCurrentSpanPath(out var path))
            SerializeTool.SerializeFile(currentSpan, new(CURRENT_SPAN), SignTable, true, path);
    }

    public static long LoadCurrentSpan()
    {
        if (GetCurrentSpanPath(out var path))
            return SerializeTool.DeserializeFile<long>(new(CURRENT_SPAN), SignTable, path);
        else
            return 0;
    }

    public static void SaveRandomTable(RandomTable randomTable)
    {
        if (GetRandomTablePath(out var path))
            SerializeTool.SerializeFile(randomTable, new(RADOM_TABLE), SignTable, true, path);
    }

    public static RandomTable LoadRandomTable()
    {
        if (GetRandomTablePath(out var path))
            return SerializeTool.DeserializeFile<RandomTable>(new(RADOM_TABLE), SignTable, path) ?? new();
        else
            return new();
    }

    public static void SaveWorldSize(Size wordSize)
    {
        if (GetWorldSizePath(out var path))
            SerializeTool.SerializeFile(wordSize, new(WORLD_SIZE), SignTable, true, path);
    }

    public static Size LoadWorldSize()
    {
        if (GetWorldSizePath(out var path))
            return SerializeTool.DeserializeFile<Size>(new(WORLD_SIZE), SignTable, path);
        else
            return new();
    }

    public static void SaveLandPoints(List<LandPoint> landPoints)
    {
        using var query = GetArchiveQuery();
        if (query is null)
            return;
        query.CreateTable<LandPoint>(LAND_POINTS);
        query.InsertItems(LAND_POINTS, landPoints.ToArray());
    }

    public static List<LandPoint> LoadLandPoints()
    {
        using var query = GetArchiveQuery();
        if (query is null)
            return [];
        return query.SelectItems<LandPoint>(LAND_POINTS, null).ToList() ?? [];
    }

    public static void InitOwnerSites()
    {
        using var query = GetArchiveQuery();
        if (query is null)
            return;
        query.CreateTable<OwnerSite>(OWNER_SITES);
    }

    public static void SetOwnerSites(Coordinate site, SourceLandTypes landType, string playerName)
    {
        using var query = GetArchiveQuery();
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
        using var query = GetArchiveQuery();
        if (query is null)
            return [];
        var ownerSite = new OwnerSite() { PlayerName = playerName };
        var condition = SQLiteQuery.GetCondition(ownerSite, Operators.Equal, nameof(OwnerSite.PlayerName));
        return query.SelectItems<OwnerSite>(OWNER_SITES, condition).ToList();
    }

    public static List<OwnerSite> GetOwnerSites()
    {
        using var query = GetArchiveQuery();
        if (query is null)
            return [];
        return query.SelectItems<OwnerSite>(OWNER_SITES, null).ToList();
    }

    public static void RemoveOwnerSite(Coordinate site)
    {
        using var query = GetArchiveQuery();
        if (query is null)
            return;
        var ownerSite = new OwnerSite() { Site = site };
        var condition = SQLiteQuery.GetCondition(ownerSite, Operators.Equal, nameof(OwnerSite.Site));
        query.DeleteItems(OWNER_SITES, condition);
    }
}
