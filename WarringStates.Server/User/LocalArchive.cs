using AltitudeMapGenerator;
using LocalUtilities.SimpleScript.Common;
using LocalUtilities.SQLiteHelper;
using LocalUtilities.TypeGeneral;
using WarringStates.Map;
using WarringStates.Server.Data;
using WarringStates.Server.Events;
using WarringStates.Server.Map;
using WarringStates.User;

namespace WarringStates.Server.User;

internal partial class LocalArchive
{
    public static string RootPath { get; } = Directory.CreateDirectory(nameof(Archive)).FullName;

    //static string RegisterPath { get; } = Path.Combine(RootPath, nameof(ArchiveInfoList) + ".db");

    public static ArchiveInfoRoster Archives { get; } = [];

    static SsSignTable SignTable { get; } = new();

    public static void Relocate()
    {
        try
        {
            using var query = LocalDataBase.NewQuery();
            Relocate(query);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private static void Relocate(SQLiteQuery query)
    {
        Archives.Clear();
        foreach (var info in query.SelectItems<ArchiveInfo>(LocalDataBase.NameofArchive, null))
            Archives.TryAdd(info);
        LocalEvents.TryBroadcast(LocalEvents.UserInterface.ArchiveListRefreshed);
    }

    public static void CreateArchive(AltitudeMapData mapData, string worldName, IProgressor progressor)
    {
        try
        {
            var info = new ArchiveInfo(worldName);
            Directory.CreateDirectory(Path.Combine(RootPath, info.Id));
            var altitudeMap = new AltitudeMap(mapData, progressor);
            var randomTable = new RandomTable(1000);
            var landPoints = LandMapEx.ConvertLandPoints(altitudeMap);
            var landMap = new LandMapEx(altitudeMap.Size, randomTable, landPoints);
            SaveCurrentSpan(info, 0);
            SaveRandomTable(info, randomTable);
            SaveWorldSize(info, landMap.WorldSize);
            SaveLandPoints(info, landPoints);
            InitOwnerSites(info);
            SaveThumbnail(info, landMap);
            using var query = LocalDataBase.NewQuery();
            query.CreateTable<ArchiveInfo>(LocalDataBase.NameofArchive);
            query.InsertItem(LocalDataBase.NameofArchive, info);
            Relocate(query);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    public static bool Delete(int index)
    {
        try
        {
            if (!Archives.TryGetValue(index, out var info))
                return false;
            if (MessageBox.Show($"要永远删除 {info.WorldName} 吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                return false;
            try
            {
                Directory.Delete(GetArchiveRootPath(info), true);
            }
            catch { }
            using var query = LocalDataBase.NewQuery();
            query.DeleteItems(LocalDataBase.NameofArchive, SQLiteQuery.GetCondition(info, Operators.Equal));
            Relocate(query);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            return false;
        }
    }

    public static PlayerArchive GetPlayerArchive(ArchiveInfo info, LandMapEx landMap, string playerId)
    {
        return new()
        {
            ArchiveId = info.Id,
            WorldName = info.WorldName,
            WorldSize = landMap.WorldSize,
            CurrentSpan = LoadCurrentSpan(info),
            VisibleLands = GetVisibleLands(info, landMap, playerId),
            //VisibleLands = landMap.GetAllSingleLands(),
        };
    }

    public static VisibleLands GetVisibleLands(ArchiveInfo info, LandMapEx landMap, string playerId)
    {
        var visibleLands = new VisibleLands();
        var ownerSites = GetOwnerSites(info, playerId);
        foreach (var ownerSite in ownerSites)
        {
            landMap.GetVision(ownerSite.Site, visibleLands);
        }
        return visibleLands;
    }

    public static LandMapEx InitializeLandMap(ArchiveInfo info)
    {
        var worldSize = LoadWorldSize(info);
        var randomTable = LoadRandomTable(info);
        var landPoints = LoadLandPoints(info);
        var landMap = new LandMapEx(worldSize, randomTable, landPoints);
        var ownerSites = GetOwnerSites(info);
        foreach (var ownerSite in ownerSites)
        {
            if (!landMap.AddSouceLand(ownerSite.Site, ownerSite.LandType))
                RemoveOwnerSite(info, ownerSite.Site);
        }
        return landMap;
    }
}
