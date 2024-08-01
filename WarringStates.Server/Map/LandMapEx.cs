using AltitudeMapGenerator;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Graph;
using LocalUtilities.TypeToolKit.Mathematic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using WarringStates.Map;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace WarringStates.Server.Map;

internal class LandMapEx : LandMap
{
    public override Land this[Coordinate point]
    {
        get
        {
            if (SourceLands.TryGetValue(point, out var sourceLand))
                return sourceLand;
            if (SingleLands.TryGetValue(point, out var singleLand))
                return singleLand;
            return new SingleLand(point, SingleLandTypes.Plain);
        }
    }

    Dictionary<SingleLandTypes, int> LandTypesCount { get; } = [];

    public string GetLandTypeCount(SingleLandTypes type)
    {
        var total = LandTypesCount.Values.Sum();
        if (LandTypesCount.TryGetValue(type, out var count))
            return new StringBuilder()
                .Append(count)
                .Append('(')
                .Append(Math.Round(count / (double)total * 100, 2))
                .Append('%')
                .Append(')')
                .ToString();
        else
            return "0";
    }

    public LandMapEx(Size worldSize, RandomTable randomTable, List<LandPoint> landPoints)
    {
        WorldSize = worldSize;
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
        var area = WorldWidth * WorldHeight;
        LandTypesCount[SingleLandTypes.Plain] = area - LandTypesCount.Sum(x => x.Key is SingleLandTypes.Plain ? 0 : x.Value);
    }

    public LandMapEx()
    {

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

    public void RemoveSourceLand(SourceLand sourceLand)
    {
        var sites = sourceLand.GetAllSites();
    }

    public bool AddSouceLand(Coordinate site, SourceLandTypes targetType)
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="site"></param>
    /// <param name="targetType"></param>
    /// <returns>return empty if build failed</returns>
    private List<SourceLand> GetSurrounds(Coordinate site, SourceLandTypes targetType)
    {
        if (!CheckSurround(site, out var counts, out var points))
            return [];
        var canBuild = CanBuild(targetType, counts);
        if (!canBuild)
            return [];
        return points.Select(p => new SourceLand(p.Key, p.Value, targetType)).ToList();
    }

    public void GetVision(Coordinate site, VisibleLands vision)
    {
        var left = site.X - 2;
        var top = site.Y - 2;
        for (var i = 0; i < 5; i++)
        {
            for (var j = 0; j < 5; j++)
            {
                var point = SetPointWithin(new(left + i, top + j));
                vision.AddLand(this[point]);
            }
        }
    }

    public SourceLandTypes[] GetCanBuildTypes(Coordinate site)
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

    private bool CheckSurround(Coordinate site, out Dictionary<SingleLandTypes, int> counts, out Dictionary<Coordinate, Directions> points)
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
                var land = this[point];
                if (land is not SingleLand singleLand)
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

    public static List<LandPoint> ConvertLandPoints(AltitudeMap altitudeMap)
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

    [Obsolete("for test")]
    public VisibleLands GetAllSingleLands()
    {
        var lands = new VisibleLands();
        SingleLands.ToList().ForEach(x => lands.AddLand(x));
        return lands;
    }

    public OwnerSite SetRandomSite(string playerId)
    {
        var random = new Random();
        var gen = PointGenerator.GeneratePoint(random, 0, 0, WorldWidth, WorldHeight, 1);
        var site = new Coordinate(gen[0].X.ToRoundInt(), gen[0].Y.ToRoundInt());
        var type = (SourceLandTypes)(random.Next() % 8);
        var lands = GetSurrounds(site, type);
        while (lands.Count < 1)
        {
            gen = PointGenerator.GeneratePoint(random, 0, 0, WorldWidth, WorldHeight, 1);
            site = new Coordinate(gen[0].X.ToRoundInt(), gen[0].Y.ToRoundInt());
            type = (SourceLandTypes)(random.Next() % 8);
            lands = GetSurrounds(site, type);
        }
        SourceLands.AddArange(lands);
        return new(site, type, playerId);
    }
}
