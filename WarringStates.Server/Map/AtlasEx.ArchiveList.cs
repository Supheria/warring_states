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

    public static List<ArchiveInfo> Archives { get; } = [];

    public static void RefreshArchiveList()
    {
        try
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
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
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
            RefreshArchiveList();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
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

    public static bool Delete(int index)
    {
        try
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
