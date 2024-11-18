using AltitudeMapGenerator;
using LocalUtilities.SimpleScript;
using LocalUtilities.SQLiteHelper;
using LocalUtilities.TypeGeneral;
using System.Diagnostics.CodeAnalysis;
using WarringStates.Server.Data;
using WarringStates.Server.Events;
using WarringStates.User;

namespace WarringStates.Server.Map;

partial class AtlasEx
{
    public static string RootPath { get; } = Directory.CreateDirectory("saves").FullName;

    public static ArchiveInfo? CurrentArchiveInfo { get; private set; } = null;

    public static ArchiveInfoRoster Archives { get; } = [];

    public static void RefreshArchiveList()
    {
        try
        {
            using var query = LocalDataBase.NewQuery();
            RefreshArchiveList(query);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private static void RefreshArchiveList(SQLiteQuery query)
    {
        Archives.Clear();
        foreach (var info in query.SelectItems<ArchiveInfo>(LocalDataBase.ARCHIVE_INFO, null))
            Archives.TryAdd(info);
        LocalEvents.TryBroadcast(LocalEvents.UserInterface.ArchiveListRefreshed);
    }

    public static void CreateArchive(AltitudeMapData mapData, string worldName, IProgressor progressor)
    {
        try
        {
            CurrentArchiveInfo = new(worldName);
            //SetCurrentArchive(new ArchiveInfo(worldName));
            var altitudeMap = new AltitudeMap(mapData, progressor);
            var landPoints = AtlasEx.ConvertLandPoints(altitudeMap);
            if (!GetArchiveRootPath(out var rootPath) ||
                !GetCurrentSpanPath(out var spanPath) ||
                !GetRandomTablePath(out var randomPath) ||
                !GetWorldSizePath(out var sizePath))
                return;
            Directory.CreateDirectory(rootPath);
            SerializeTool.SerializeFile(0, new(CURRENT_SPAN), SignTable, true, spanPath);
            SerializeTool.SerializeFile(new RandomTable(1000), new(RADOM_TABLE), SignTable, true, randomPath);
            SerializeTool.SerializeFile(altitudeMap.Size, new(WORLD_SIZE), SignTable, true, sizePath);
            using var query = GetArchiveQuery();
            if (query is null)
                return;
            query.CreateTable<LandPoint>(LAND_POINTS);
            query.InsertItems(LAND_POINTS, landPoints.ToArray());
            query.CreateTable<OwnerSite>(OWNER_SITES);
            query.CreateTable<ArchiveInfo>(LocalDataBase.ARCHIVE_INFO);
            query.InsertItem(LocalDataBase.ARCHIVE_INFO, CurrentArchiveInfo);
            using var mainQuery = LocalDataBase.NewQuery();
            mainQuery.CreateTable<ArchiveInfo>(LocalDataBase.ARCHIVE_INFO);
            mainQuery.InsertItem(LocalDataBase.ARCHIVE_INFO, CurrentArchiveInfo);
            RefreshArchiveList(mainQuery);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    public static void SetCurrentArchive(int index)
    {
        Archives.TryGetValue(index, out var info);
        SetCurrentArchive(info);
    }

    private static void SetCurrentArchive(ArchiveInfo? info)
    {
        CurrentArchiveInfo = info;
        if (CurrentArchiveInfo is null)
            AtlasEx.Relocate();
        else
        {
            AtlasEx.Relocate(LoadWorldSize(), LoadLandPoints(), LoadRandomTable());
            using var query = GetArchiveQuery();
            var ownerSites = query?.SelectItems<OwnerSite>(OWNER_SITES, null).ToList() ?? [];
            foreach (var ownerSite in ownerSites)
            {
                if (!AtlasEx.AddSouceLand(ownerSite.Site, ownerSite.LandType))
                    RemoveOwnerSite(ownerSite.Site);
            }
        }
        LocalEvents.TryBroadcast(LocalEvents.UserInterface.CurrentArchiveChange);
    }

    public static bool Delete(int index)
    {
        try
        {
            if (!Archives.TryGetValue(index, out var info))
                return false;
            if (MessageBox.Show($"要永远删除 {info.WorldName} 吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                return false;
            if (GetArchiveRootPath(out var path))
                Directory.Delete(path, true);
            using var query = LocalDataBase.NewQuery();
            query.DeleteItems(LocalDataBase.ARCHIVE_INFO, SQLiteQuery.GetCondition(info, Operators.Equal));
            RefreshArchiveList(query);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            return false;
        }
    }

    public static bool GetPlayerArchive(string playerName, [NotNullWhen(true)] out PlayerArchive? playerArchive)
    {
        playerArchive = null;
        if (CurrentArchiveInfo is null)
            return false;
        playerArchive = new()
        {
            ArchiveId = CurrentArchiveInfo.Id,
            WorldName = CurrentArchiveInfo.WorldName,
            WorldSize = LoadWorldSize(),
            CurrentSpan = LoadCurrentSpan(),
            VisibleLands = AtlasEx.GetAllVision(playerName),
            //VisibleLands = LandMap.GetAllSingleLands(),
        };
        return true;
    }
}
