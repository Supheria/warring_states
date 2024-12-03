using Avalonia.Media.Imaging;
using LocalUtilities;
using LocalUtilities.General;
using LocalUtilities.GUICore;
using LocalUtilities.SQLiteHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using WarringStates.Map;
using WarringStates.User;

namespace WarringStates.Server.GUI.Models;

internal partial class AtlasEx : Atlas
{
    public static string RootPath { get; } = Directory.CreateDirectory("saves").FullName;

    protected static Roster<Coordinate, SingleLand> SingleLands { get; } = [];

    static Dictionary<SingleLandTypes, int> LandTypesCount { get; } = [];

    public static SingleLand GetSingleLand(Coordinate site)
    {
        if (SingleLands.TryGetValue(site, out var singleLand))
            return singleLand;
        return new SingleLand(site, SingleLandTypes.Plain);
    }

    public static bool AddPlayer(Player player)
    {
        if (CurrentArchiveInfo is null)
            return false;
        using var query = GetPlayerDatabaseQuery(CurrentArchiveInfo);
        query.Begin();
        var tableName = player.GetNameHash();
        query.CreateTable<VisibleSite>(tableName);
        var center = new VisibleSite() { Direction = Directions.Center };
        var condition = SQLiteQuery.GetCondition(center, Operators.Equal, nameof(VisibleSite.Direction));
        if (query.SelectItems<VisibleSite>(player.GetNameHash(), condition).Length is not 0)
            return true;
        query.Commit();
        RandomGenerator.Reset();
        var (x, y) = RandomGenerator.GeneratePoint(0, 0, Width, Height);
        var site = new Coordinate(x.ToRoundInt(), y.ToRoundInt());
        var types = GetCanBuildTypes(site);
        if (types.Length is 0)
            return false;
        return CreateSourceLand(player, site, types[new Random().Next() % types.Length]);
    }

    public static VisibleLands GetSiteVision(Coordinate site, Player player)
    {
        if (CurrentArchiveInfo is null)
            return new();
        using var query = GetPlayerDatabaseQuery(CurrentArchiveInfo);
        query.Begin();
        var tableName = player.GetNameHash();
        VisibleSite? visibleSite = new() { Site = site };
        var condition = SQLiteQuery.GetCondition(visibleSite, Operators.Equal, nameof(VisibleSite.Site));
        visibleSite = query.SelectItems<VisibleSite>(tableName, condition).FirstOrDefault();
        var vision = new VisibleLands();
        if (visibleSite is null || visibleSite.SourceType is SourceLandTypes.None)
        {
            vision.Add(GetSingleLand(site));
            return vision;
        }
        var center = visibleSite.GetCenter();
        for (var i = -2; i < 3; i++)
        {
            for (var j = -2; j < 3; j++)
            {
                visibleSite = new() { Site = SetPointWithin(center + (i, j)) };
                condition = SQLiteQuery.GetCondition(visibleSite, Operators.Equal, nameof(VisibleSite.Site));
                visibleSite = query.SelectItems<VisibleSite>(tableName, condition).FirstOrDefault();
                if (visibleSite is null)
                    continue;
                if (visibleSite.SourceType is SourceLandTypes.None)
                    vision.Add(GetSingleLand(visibleSite.Site));
                else
                    vision.Add(new SourceLand(visibleSite.Site, visibleSite.SourceType, visibleSite.Direction));
            }
        }
        return vision;
    }

    public static bool CreateSourceLand(Player player, Coordinate center, SourceLandTypes type)
    {
        if (CurrentArchiveInfo is null)
            return false;
        var sites = GetSourceLandSites(center, type);
        if (sites.Length is not 9)
            return false;
        using var query = GetPlayerDatabaseQuery(CurrentArchiveInfo);
        query.Begin();
        var tableName = player.GetNameHash();
        query.InsertItems(tableName, sites.ToArray(), InsertTypes.ReplaceIfExists);
        var visibles = GetSourceLandSurroundVision(CurrentArchiveInfo, player, center);
        query.InsertItems(tableName, visibles.ToArray(), InsertTypes.IgnoreIfExists);
        return true;
    }

    public static List<VisibleSite> GetSourceLandSurroundVision(ArchiveInfo archiveInfo, Player player, Coordinate center)
    {
        var visibles = new List<VisibleSite>();
        for (var i = -1; i < 2; i++)
        {
            visibles.AddRange([
                new(SetPointWithin(center + (-2, i))),
                new(SetPointWithin(center + (2, i))),
                new(SetPointWithin(center + (i, -2))),
                new(SetPointWithin(center + (i, 2)))
                ]);
        }
        visibles.AddRange([
            new(SetPointWithin(center + (-2, -2))),
            new(SetPointWithin(center + (2, -2))),
            new(SetPointWithin(center + (-2, 2))),
            new(SetPointWithin(center + (2, 2))),
            ]);
        return visibles;
    }

    public static SourceLandTypes[] GetCanBuildTypes(Coordinate site)
    {
        if (!CheckSurround(site, out var counts, out var points))
            return [];
        var result = new List<SourceLandTypes>();
        foreach (var type in Enum.GetValues<SourceLandTypes>())
        {
            if (CanBuild(type, counts))
                result.Add(type);
        }
        return result.ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="site"></param>
    /// <param name="targetType"></param>
    /// <returns>return empty if build failed</returns>
    private static VisibleSite[] GetSourceLandSites(Coordinate site, SourceLandTypes targetType)
    {
        if (!CheckSurround(site, out var counts, out var surrounds))
            return [];
        var canBuild = CanBuild(targetType, counts);
        if (!canBuild)
            return [];
        return surrounds.Select(s => new VisibleSite(s.Key, targetType, s.Value)).ToArray();
    }

    private static bool CanBuild(SourceLandTypes type, Dictionary<SingleLandTypes, int> counts)
    {
        return type switch
        {
            SourceLandTypes.HorseLand => counts[SingleLandTypes.Plain] + counts[SingleLandTypes.Stream] is 9
            && Math.Min(counts[SingleLandTypes.Plain], counts[SingleLandTypes.Stream]) is not 0,
            SourceLandTypes.MineLand => counts[SingleLandTypes.Wood] + counts[SingleLandTypes.Hill] is 9
            && Math.Min(counts[SingleLandTypes.Wood], counts[SingleLandTypes.Hill]) is not 0,
            SourceLandTypes.FarmLand => counts[SingleLandTypes.Plain] > 3,
            SourceLandTypes.MulberryLand => counts[SingleLandTypes.Plain] > 3,
            SourceLandTypes.WoodLand => counts[SingleLandTypes.Wood] > 3,
            SourceLandTypes.FishLand => counts[SingleLandTypes.Stream] > 3,
            SourceLandTypes.TerraceLand => counts[SingleLandTypes.Hill] > 3,
            _ => false
        };
    }

    public static bool CheckSurround(Coordinate center, out Dictionary<SingleLandTypes, int> counts, out Dictionary<Coordinate, Directions> sites)
    {
        sites = [];
        counts = new()
        {
            [SingleLandTypes.Plain] = 0,
            [SingleLandTypes.Stream] = 0,
            [SingleLandTypes.Wood] = 0,
            [SingleLandTypes.Hill] = 0,
        };
        if (CurrentArchiveInfo is null)
            return false;
        var directionOrder = 0;
        for (var i = -1; i < 2; i++)
        {
            for (var j = -1; j < 2; j++)
            {
                var site = SetPointWithin(center + (i, j));
                var exist = GetVisibleSite(CurrentArchiveInfo, site);
                if (exist is not null && exist.SourceType is not SourceLandTypes.None)
                    return false;
                counts[GetSingleLand(site).Type]++;
                sites[site] = directionOrder switch
                {
                    0 => Directions.LeftTop,
                    1 => Directions.Left,
                    2 => Directions.LeftBottom,
                    3 => Directions.Top,
                    4 => Directions.Center,
                    5 => Directions.Bottom,
                    6 => Directions.TopRight,
                    7 => Directions.Right,
                    8 => Directions.BottomRight,
                    _ => Directions.None,
                };
                directionOrder++;
            }
        }
        return true;
    }

    public static VisibleSite? GetVisibleSite(ArchiveInfo archiveInfo, Coordinate site)
    {
        using var query = GetPlayerDatabaseQuery(archiveInfo);
        query.Begin();
        var tableNames = query.ListAllTableNames();
        foreach (var table in tableNames)
        {
            var v = new VisibleSite() { Site = site };
            var condition = SQLiteQuery.GetCondition(v, Operators.Equal, nameof(VisibleSite.Site));
            var visible = query.SelectItems<VisibleSite>(table, condition).FirstOrDefault();
            if (visible is not null)
                return visible;
        }
        return null;
    }

    public static Bitmap? GetThumbnail()
    {
        if (CurrentArchiveInfo is null || Width is 0 || Height is 0)
            return null;
        var thumbnail = new WriteableBitmap(new(Width, Height), new(96, 96));
        using var lockedBuffer = thumbnail.Lock();
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                var color = GetSingleLand(new(i, j)).Color;
                lockedBuffer.SetPixel(i, j, color);
            }
        }
        var cache = new MemoryStream();
        thumbnail.Save(cache);
        cache.Position = 0;
        return new Bitmap(cache);
    }

    public static bool GetAtlasData(Player player, [NotNullWhen(true)] out AtlasData? data)
    {
        data = null;
        if (CurrentArchiveInfo is null)
            return false;
        data = new()
        {
            WorldSize = CurrentArchiveInfo.WorldSize,
        };
        using var query = GetPlayerDatabaseQuery(CurrentArchiveInfo);
        var sites = query.SelectItems<VisibleSite>(player.GetNameHash(), null);
        foreach (var site in sites)
        {
            Land land;
            if (site.SourceType is SourceLandTypes.None)
                land = GetSingleLand(site.Site);
            else
                land = new SourceLand(site.Site, site.SourceType, site.Direction);
            data.VisibleLands.Add(land);
        }
        return true;
    }
}
