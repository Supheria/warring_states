using AltitudeMapGenerator;
using LocalUtilities.SimpleScript;
using LocalUtilities.SQLiteHelper;
using LocalUtilities.TypeGeneral;
using System.Diagnostics.CodeAnalysis;
using WarringStates.Map;
using WarringStates.Server.Data;
using WarringStates.Server.Events;
using WarringStates.User;

namespace WarringStates.Server.Map;

partial class AtlasEx
{
    static ArchiveInfo? CurrentArchiveInfo { get; set; } = null;

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
            CurrentArchiveInfo = new(worldName, mapData.Size);
            var altitudeMap = new AltitudeMap(mapData, progressor);
            using var query = CurrentArchiveInfo.GetQuery();
            query.CreateTable<LandPoint>(LAND_POINTS);
            query.InsertItems(LAND_POINTS, ConvertLandPoints(altitudeMap).ToArray());
            query.CreateTable<OwnerSite>(OWNER_SITES);

            // TODO: remove archiveinfo list in local.db
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

    public static bool Delete(int index)
    {
        try
        {
            if (!Archives.TryGetValue(index, out var info))
                return false;
            if (MessageBox.Show($"要永远删除 {info.WorldName} 吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                return false;
            File.Delete(info.GetDatabase());
            // TODO:
            //RefreshArchiveList(query);
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
            WorldSize = CurrentArchiveInfo.WorldSize,
            CurrentSpan = LoadCurrentSpan(),
            VisibleLands = GetAllVision(playerName),
            //VisibleLands = GetAllSingleLands(),
        };
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
