using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Graph;
using LocalUtilities.TypeToolKit.Mathematic;
using System.Diagnostics.CodeAnalysis;
using WarringStates.Map;

namespace WarringStates.Server.Map;

internal partial class AtlasEx : Atlas
{
    static Dictionary<SingleLandTypes, int> LandTypesCount { get; } = [];

    public static bool AddSouceLand(Coordinate site, SourceLandTypes targetType)
    {
        var surrounds = GetSurrounds(site, targetType);
        if (surrounds.Count is not 9)
            return false;
        foreach (var land in surrounds)
        {
            if (!SourceLands.TryAdd(land))
            {
                surrounds.ForEach(s => SourceLands.TryRemove(s));
                return false;
            }
        }
        return true;
    }

    public static Land GetLand(Coordinate point)
    {
        if (SourceLands.TryGetValue(point, out var sourceLand))
            return sourceLand;
        if (SingleLands.TryGetValue(point, out var singleLand))
            return singleLand;
        return new SingleLand(point, SingleLandTypes.Plain);
    }

    public static OwnerSite SetRandomSite(string playerName)
    {
        var random = new Random();
        var gen = PointGenerator.GeneratePoint(random, 0, 0, Width, Height, 1);
        var site = new Coordinate(gen[0].X.ToRoundInt(), gen[0].Y.ToRoundInt());
        var type = (SourceLandTypes)(random.Next() % 8);
        var lands = GetSurrounds(site, type);
        while (lands.Count < 1)
        {
            gen = PointGenerator.GeneratePoint(random, 0, 0, Width, Height, 1);
            site = new Coordinate(gen[0].X.ToRoundInt(), gen[0].Y.ToRoundInt());
            type = (SourceLandTypes)(random.Next() % 8);
            lands = GetSurrounds(site, type);
        }
        SourceLands.AddArange(lands);
        var owner = new OwnerSite(site, type, playerName);
        SetOwnerSites(owner.Site, owner.LandType, playerName);
        return owner;
    }

    public static void GetVision(Coordinate site, VisibleLands vision)
    {
        var left = site.X - 2;
        var top = site.Y - 2;
        for (var i = 0; i < 5; i++)
        {
            for (var j = 0; j < 5; j++)
            {
                var point = SetPointWithin(new(left + i, top + j));
                vision.AddLand(GetLand(point));
            }
        }
    }

    public static VisibleLands GetSiteVision(Coordinate site)
    {
        var vision = new VisibleLands();
        GetVision(site, vision);
        return vision;
    }

    public static VisibleLands GetAllVision(string playerName)
    {
        var visibleLands = new VisibleLands();
        var ownerSites = GetOwnerSites(playerName);
        foreach (var ownerSite in ownerSites)
        {
            GetVision(ownerSite.Site, visibleLands);
        }
        return visibleLands;
    }

    public static bool BuildSourceLand(Coordinate site, SourceLandTypes type, string playerName)
    {
        var surrounds = GetSurrounds(site, type);
        if (surrounds.Count is not 9)
            return false;
        foreach (var land in surrounds)
        {
            if (!SourceLands.TryAdd(land))
            {
                surrounds.ForEach(s => SourceLands.TryRemove(s));
                return false;
            }
        }
        SetOwnerSites(site, type, playerName);
        return true;
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
    private static List<SourceLand> GetSurrounds(Coordinate site, SourceLandTypes targetType)
    {
        if (!CheckSurround(site, out var counts, out var points))
            return [];
        var canBuild = CanBuild(targetType, counts);
        if (!canBuild)
            return [];
        return points.Select(p => new SourceLand(p.Key, p.Value, targetType)).ToList();
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

    public static bool CheckSurround(Coordinate site, out Dictionary<SingleLandTypes, int> counts, out Dictionary<Coordinate, Directions> points)
    {
        points = [];
        counts = new()
        {
            [SingleLandTypes.Plain] = 0,
            [SingleLandTypes.Stream] = 0,
            [SingleLandTypes.Wood] = 0,
            [SingleLandTypes.Hill] = 0,
        };
        var left = site.X - 1;
        var top = site.Y - 1;
        var directionOrder = 0;
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                var point = SetPointWithin(new(left + i, top + j));
                if (GetLand(point) is not SingleLand singleLand)
                    return false;
                points[point] = directionOrder switch
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
                counts[singleLand.LandType]++;
            }
        }
        return true;
    }

    public static Bitmap? GetThumbnail()
    {
        if (Width is 0 || Height is 0)
            return null;
        var thumbnail = new Bitmap(Width, Height);
        var pThumbnail = new PointBitmap(thumbnail);
        pThumbnail.LockBits();
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                var color = GetLand(new(i, j)).Color;
                pThumbnail.SetPixel(i, j, color);
            }
        }
        pThumbnail.UnlockBits();
        return thumbnail;
    }

    public static bool GetAtlasData(string playerName, [NotNullWhen(true)] out AtlasData? playerArchive)
    {
        playerArchive = null;
        if (CurrentArchiveInfo is null)
            return false;
        playerArchive = new()
        {
            WorldSize = CurrentArchiveInfo.WorldSize,
            VisibleLands = GetAllVision(playerName),
            //VisibleLands = GetAllSingleLands(),
        };
        return true;
    }
}
