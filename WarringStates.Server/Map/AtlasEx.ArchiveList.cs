using AltitudeMapGenerator;
using LocalUtilities.SQLiteHelper;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using WarringStates.Map;
using WarringStates.Server.Events;

namespace WarringStates.Server.Map;

partial class AtlasEx
{
    public const string LAND_POINTS = "land points";

    static ArchiveInfo? CurrentArchiveInfo { get; set; } = null;

    public static List<ArchiveInfo> Archives { get; } = [];

    public static long CurrentSpan
    {
        get => CurrentArchiveInfo?.CurrentSpan ?? 0;
        set => CurrentArchiveInfo?.UpdateCurrentSpan(value);
    }

    public static void RefreshArchiveList()
    {
        Archives.Clear();
        foreach (var path in Directory.EnumerateFiles(ArchiveInfo.RootPath))
        {
            SQLiteQuery? query = null;
            try
            {
                query = new SQLiteQuery(path);
                var archiveInfo = query.SelectItems<ArchiveInfo>(ArchiveInfo.ARCHIVE_INFO, null).FirstOrDefault();
                if (archiveInfo is not null)
                    Archives.Add(archiveInfo);
            }
            catch { }
            finally
            {
                query?.Dispose();
            }
        }
        LocalEvents.TryBroadcast(LocalEvents.UserInterface.ArchiveListRefreshed);
    }

    public static void CreateArchive(AltitudeMapData mapData, string worldName, IProgressor progressor)
    {
        CurrentArchiveInfo = new(worldName, mapData.Size);
        var altitudeMap = new AltitudeMap(mapData, progressor);
        using var query = CurrentArchiveInfo.GetQuery();
        query.CreateTable<LandPoint>(LAND_POINTS);
        query.InsertItems(LAND_POINTS, ConvertToLandPoints(altitudeMap).ToArray());
        query.CreateTable<OwnerSite>(OWNER_SITES);
        RefreshArchiveList();
    }

    public static List<LandPoint> ConvertToLandPoints(AltitudeMap altitudeMap)
    {
        var landPoints = new Dictionary<Coordinate, LandPoint>();
        foreach (var (coordinate, point) in altitudeMap.AltitudePoints)
        {
            var site = SetPointWithin(coordinate, altitudeMap.Size);
            landPoints[site] = new(site, point.Altitude / altitudeMap.AltitudeMax, PointTypes.Normal);
        }
        foreach (var coordinate in altitudeMap.RiverPoints)
        {
            var site = SetPointWithin(coordinate, altitudeMap.Size);
            if (landPoints.TryGetValue(site, out var point))
                landPoints[site] = new(site, point.AltitudeRatio, PointTypes.River);
            else
                landPoints[site] = new(site, 0d, PointTypes.River);
        }
        foreach (var coordinate in altitudeMap.OriginPoints)
        {
            var site = SetPointWithin(coordinate, altitudeMap.Size);
            if (landPoints.TryGetValue(site, out var point))
                landPoints[site] = new(site, point.AltitudeRatio, PointTypes.Origin);
            else
                landPoints[site] = new(site, 1d, PointTypes.Origin);
        }
        return landPoints.Values.ToList();
    }

    public static void SetCurrentArchive(int index)
    {
        if (index < 0 || index >= Archives.Count)
            return;
        CurrentArchiveInfo = Archives[index];
        Relocate();
        using var query = CurrentArchiveInfo?.GetQuery();
        if (query is null)
            return;
        var ownerSites = query.SelectItems<OwnerSite>(OWNER_SITES, null).ToList() ?? [];
        foreach (var ownerSite in ownerSites)
        {
            if (!AddSouceLand(ownerSite.Site, ownerSite.LandType))
                RemoveOwnerSite(ownerSite.Site);
        }
        LocalEvents.TryBroadcast(LocalEvents.UserInterface.CurrentArchiveChange);
    }

    public static void Relocate()
    {
        SingleLands.Clear();
        SourceLands.Clear();
        Size = new();
        if (CurrentArchiveInfo is null)
            return;
        Size = CurrentArchiveInfo.WorldSize;
        var randomTable = CurrentArchiveInfo.RandomTable;
        randomTable.ResetIndex();
        using var query = CurrentArchiveInfo.GetQuery();
        var landPoints = query.SelectItems<LandPoint>(LAND_POINTS, null).ToList();
        foreach (var point in landPoints)
        {
            SingleLandTypes type;
            if (point.Type is PointTypes.River)
                type = SingleLandTypes.Stream;
            else
                type = AltitudeFilter(point.AltitudeRatio, randomTable.Next());
            var singleLand = new SingleLand(point.Coordinate, type);
            SingleLands[point.Coordinate] = singleLand;
            if (LandTypesCount.TryGetValue(type, out var value))
                LandTypesCount[type] = ++value;
            else
                LandTypesCount[type] = 1;
        }
        var area = Width * Height;
        LandTypesCount[SingleLandTypes.Plain] = area - LandTypesCount.Sum(x => x.Key is SingleLandTypes.Plain ? 0 : x.Value);
    }

    private static SingleLandTypes AltitudeFilter(double altitudeRatio, double random)
    {
        if (altitudeRatio.ApproxLessThan(0.05))
        {
            if (random.ApproxLessThan(0.33))
                return SingleLandTypes.Plain;
            if (random.ApproxLessThan(0.9))
                return SingleLandTypes.Wood;
        }
        else if (altitudeRatio.ApproxLessThan(0.15))
        {
            if (random.ApproxLessThan(0.33))
                return SingleLandTypes.Wood;
            if (random.ApproxLessThan(0.95))
                return SingleLandTypes.Hill;
        }
        else
        {
            if (random.ApproxLessThan(0.8))
                return SingleLandTypes.Hill;
            if (random.ApproxLessThan(0.99))
                return SingleLandTypes.Wood;
        }
        return SingleLandTypes.Stream;
    }

    public static bool Delete(int index)
    {
        if (index < 0 || index >= Archives.Count)
            return false;
        var info = Archives[index];
        if (MessageBox.Show($"要永远删除 {info.WorldName} 吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
            return false;
        File.Delete(info.GetDatabase());
        RefreshArchiveList();
        return true;
    }

    [Obsolete("for test")]
    protected static VisibleLands GetAllSingleLands()
    {
        var lands = new VisibleLands();
        SingleLands.ToList().ForEach(x => lands.AddLand(x));
        return lands;
    }
}
