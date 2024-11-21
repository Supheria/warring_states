using AltitudeMapGenerator;
using LocalUtilities.SimpleScript;
using LocalUtilities.SQLiteHelper;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using System.Drawing;
using WarringStates.Map;
using WarringStates.Server.Events;

namespace WarringStates.Server.Map;

partial class AtlasEx
{
    public const string LAND_POINTS = "land points";

    static ArchiveInfo? CurrentArchiveInfo { get; set; } = null;

    public static List<ArchiveInfo> Archives { get; } = [];

    public static void RefreshArchiveList()
    {
        Archives.Clear();
        foreach (var folder in new DirectoryInfo(RootPath).GetDirectories()) 
        {
            try
            {
                var archiveId = folder.Name;
                var archiveInfo = LoadArchiveInfo(archiveId);
                if (archiveInfo is not null &&
                    archiveInfo.Id == archiveId)
                    Archives.Add(archiveInfo);
            }
            catch { }
        }
        LocalEvents.TryBroadcast(LocalEvents.UserInterface.ArchiveListRefreshed);
    }

    public static void CreateArchive(AltitudeMapData mapData, string worldName, IProgressor progressor)
    {
        var archiveInfo = new ArchiveInfo(worldName, mapData.Size);
        Directory.CreateDirectory(GetFolderPath(archiveInfo.Id));
        SaveArchiveInfo(archiveInfo);
        var altitudeMap = new AltitudeMap(mapData, progressor);
        SaveLandPoints(archiveInfo, ConvertToLandPoints(altitudeMap));
        RefreshArchiveList();
    }

    public static List<LandPoint> ConvertToLandPoints(AltitudeMap altitudeMap)
    {
        var random = new RandomTable(1000);
        var landPoints = new Dictionary<Coordinate, LandPoint>();
        foreach (var (coordinate, point) in altitudeMap.AltitudePoints)
        {
            var site = SetPointWithin(coordinate, altitudeMap.Size);
            var type = AltitudeFilter(point.Altitude / altitudeMap.AltitudeMax, random.Next());
            landPoints[site] = new(site, type);
        }
        foreach (var coordinate in altitudeMap.RiverPoints)
        {
            var site = SetPointWithin(coordinate, altitudeMap.Size);
            landPoints[site] = new(site, SingleLandTypes.Stream);
        }
        foreach (var coordinate in altitudeMap.OriginPoints)
        {
            var site = SetPointWithin(coordinate, altitudeMap.Size);
            landPoints[site] = new(site, SingleLandTypes.Hill);
        }
        return landPoints.Values.ToList();
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

    public static void SetCurrentArchive(int index)
    {
        if (index < 0 || index >= Archives.Count)
            return;
        SingleLands.Clear();
        SourceLands.Clear();
        Size = new();
        CurrentArchiveInfo = Archives[index];
        if (CurrentArchiveInfo is null)
            return;
        foreach (var point in LoadLandPoints(CurrentArchiveInfo))
        {
            SingleLands[point.Site] = new(point.Site, point.Type);
            if (LandTypesCount.TryGetValue(point.Type, out var value))
                LandTypesCount[point.Type] = ++value;
            else
                LandTypesCount[point.Type] = 1;
        }
        Size = CurrentArchiveInfo.WorldSize;
        var area = Width * Height;
        LandTypesCount[SingleLandTypes.Plain] = area - LandTypesCount.Sum(x => x.Key is SingleLandTypes.Plain ? 0 : x.Value);
        LocalEvents.TryBroadcast(LocalEvents.UserInterface.CurrentArchiveChange);
    }
    public static bool Delete(int index)
    {
        if (index < 0 || index >= Archives.Count)
            return false;
        var info = Archives[index];
        if (MessageBox.Show($"要永远删除 {info.WorldName} 吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
            return false;
        Directory.Delete(GetFolderPath(info.Id), true);
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
