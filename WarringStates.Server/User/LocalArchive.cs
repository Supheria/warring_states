using AltitudeMapGenerator;
using LocalUtilities.IocpNet.Common;
using LocalUtilities.SimpleScript;
using LocalUtilities.SimpleScript.Common;
using LocalUtilities.SQLiteHelper;
using LocalUtilities.SQLiteHelper.Data;
using LocalUtilities.TypeGeneral;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using WarringStates.Map;
using WarringStates.Net.Common;
using WarringStates.Server.Events;
using WarringStates.Server.Map;
using WarringStates.Server.Net;
using WarringStates.User;

namespace WarringStates.Server.User;

internal partial class LocalArchive
{
    public static string RootPath { get; } = Directory.CreateDirectory(nameof(Archive)).FullName;

    //static string RegisterPath { get; } = Path.Combine(RootPath, nameof(ArchiveInfoList) + ".db");

    public static ArchiveInfoRoster Archives { get; } = [];

    static string TableName { get; } = nameof(Archive);

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
        foreach (var info in query.SelectItems<ArchiveInfo>(TableName, null))
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
            var landPoints = LandMap.ConvertLandPoints(altitudeMap);
            var landMap = new LandMap(altitudeMap.Size, randomTable, landPoints);
            SaveCurrentSpan(info, 0);
            SaveRandomTable(info, randomTable);
            SaveWorldSize(info, landMap.WorldSize);
            SaveLandPoints(info, landPoints);
            InitOwnerSites(info);
            SaveThumbnail(info, landMap);
            using var query = LocalDataBase.NewQuery();
            query.CreateTable<ArchiveInfo>(TableName);
            query.InsertItem(TableName, info);
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
            query.DeleteItems(TableName, SQLiteQuery.GetCondition(info, Operators.Equal));
            Relocate(query);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            return false;
        }
    }

    public static PlayerArchive GetPlayerArchive(ArchiveInfo info, LandMap landMap, string playerId)
    {
        return new()
        {
            ArchiveId = info.Id,
            WorldName = info.WorldName,
            WorldSize = landMap.WorldSize,
            CurrentSpan = LoadCurrentSpan(info),
            //VisibleLands = GetVisibleLands(info, landMap, playerId),
            VisibleLands = landMap.SingleLands.ToArray(),
        };
    }

    public static LandRoster<Land> GetVisibleLands(ArchiveInfo info, LandMap landMap, string playerId)
    {
        var roster = new LandRoster<Land>();
        var ownerSites = GetOwnerSites(info, playerId);
        foreach (var ownerSite in ownerSites)
        {
            var lands = landMap.GetRoundLands(ownerSite.Site);
            lands.ForEach(l => roster.TryAdd(l));
        }
        return roster;
    }

    public static LandMap InitializeLandMap(ArchiveInfo info)
    {
        var worldSize = LoadWorldSize(info);
        var randomTable = LoadRandomTable(info);
        var landPoints = LoadLandPoints(info);
        var landMap = new LandMap(worldSize, randomTable, landPoints);
        var ownerSites = GetOwnerSites(info);
        foreach (var ownerSite in ownerSites)
        {
            var sourceLands = landMap.BuildSourceLands(ownerSite.Site, ownerSite.Type);
            if (sourceLands.Count is 0)
            {
                RemoveOwnerSite(info, ownerSite.Site);
                continue;
            }
            sourceLands.ForEach(s => landMap.SourceLands.TryAdd(s));
        }
        return landMap;
    }
}
