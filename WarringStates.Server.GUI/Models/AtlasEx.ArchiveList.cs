using AltitudeMapGenerator;
using LocalUtilities.General;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WarringStates.Map;

namespace WarringStates.Server.GUI.Models;

partial class AtlasEx
{
    static ArchiveInfo? CurrentArchiveInfo { get; set; } = null;

    public static List<ArchiveInfo> ArchiveList { get; } = [];

    public static void RefreshArchiveList()
    {
        ArchiveList.Clear();
        foreach (var folder in new DirectoryInfo(RootPath).GetDirectories())
        {
            try
            {
                var archiveId = folder.Name;
                var archiveInfo = LoadArchiveInfo(archiveId);
                if (archiveInfo is not null &&
                    archiveInfo.Id == archiveId)
                    ArchiveList.Add(archiveInfo);
            }
            catch { }
        }
        LocalEvents.TryBroadcast(LocalEvents.ArchiveListRefreshed);
    }

    public static void CreateArchive(AltitudeMapData mapData, string worldName, IProgressor progressor)
    {
        var archiveInfo = new ArchiveInfo(worldName, mapData.Size);
        Directory.CreateDirectory(GetFolderPath(archiveInfo.Id));
        SaveArchiveInfo(archiveInfo);
        var altitudeMap = new AltitudeMap(mapData, progressor);
        SaveTerrainSites(archiveInfo, ConvertToLandPoints(altitudeMap));
        RefreshArchiveList();
    }

    public static List<TerrainSite> ConvertToLandPoints(AltitudeMap altitudeMap)
    {
        var random = new RandomTable(1000);
        var landPoints = new Dictionary<Coordinate, TerrainSite>();
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

    public static void SetCurrentArchive(ArchiveInfo? archiveInfo)
    {
        if (archiveInfo is null)
            return;
        SingleLands.Clear();
        Size = new();
        CurrentArchiveInfo = archiveInfo;
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
        LocalEvents.TryBroadcast(LocalEvents.CurrentArchiveChanged);
    }

    public static bool Delete(int index)
    {
        if (index < 0 || index >= ArchiveList.Count)
            return false;
        var info = ArchiveList[index];
        Directory.Delete(GetFolderPath(info.Id), true);
        RefreshArchiveList();
        return true;
    }
}
