using AltitudeMapGenerator;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using System.Text;
using WarringStates.Map;

namespace WarringStates.Server.Map;

internal class LandMap
{
    public Size WorldSize { get; set; }

    public int WorldWidth => WorldSize.Width;

    public int WorldHeight => WorldSize.Height;

    /// <summary>
    /// use <see cref="this[Coordinate]"/> to get <see cref="Land"/>, rather than use <see cref="SingleLands"/> directly
    /// </summary>
    public LandRoster<SingleLand> SingleLands { get; } = [];
    
    public LandRoster<SourceLand> SourceLands { get; } = [];

    Dictionary<LandTypes, int> LandTypesCount { get; } = [];

    public Land this[Coordinate point]
    {
        get
        {
            if (SourceLands.TryGetValue(point, out var sourceLand))
                return sourceLand;
            if (SingleLands.TryGetValue(point, out var singleLand))
                return singleLand;
            return new SingleLand(point, LandTypes.Plain);
        }
    }

    public string GetLandTypeCount(LandTypes type)
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

    internal LandMap(Size worldSize, RandomTable randomTable, List<LandPoint> landPoints)
    {
        WorldSize = worldSize;
        foreach (var point in landPoints)
        {
            LandTypes type;
            if (point.Type is PointTypes.River)
                type = LandTypes.Stream;
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
        LandTypesCount[LandTypes.Plain] = area - LandTypesCount.Sum(x => x.Key is LandTypes.Plain ? 0 : x.Value);
    }

    private static LandTypes AltitudeFilter(double altitudeRatio, double random)
    {
        if (altitudeRatio.ApproxLessThan(0.05))
        {
            if (random.ApproxLessThan(0.33))
                return LandTypes.Plain;
            if (random.ApproxLessThan(0.9))
                return LandTypes.Wood;
        }
        else if (altitudeRatio.ApproxLessThan(0.15))
        {
            if (random.ApproxLessThan(0.33))
                return LandTypes.Wood;
            if (random.ApproxLessThan(0.95))
                return LandTypes.Hill;
        }
        else
        {
            if (random.ApproxLessThan(0.8))
                return LandTypes.Hill;
            if (random.ApproxLessThan(0.99))
                return LandTypes.Wood;
        }
        return LandTypes.Stream;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="site"></param>
    /// <param name="targetType"></param>
    /// <returns>return empty if build failed</returns>
    public List<SourceLand> BuildSourceLands(Coordinate site, LandTypes targetType)
    {
        if (!GetTerrains(site, out var counts, out var points))
            return [];
        var canBuild = targetType switch
        {
            LandTypes.HorseLand => counts[LandTypes.Plain] + counts[LandTypes.Stream] is 9,
            LandTypes.MineLand => counts[LandTypes.Wood] + counts[LandTypes.Hill] is 9,
            LandTypes.FarmLand => counts[LandTypes.Plain] > 3,
            LandTypes.MulberryLand => counts[LandTypes.Plain] > 3,
            LandTypes.WoodLand => counts[LandTypes.Wood] > 3,
            LandTypes.FishLand => counts[LandTypes.Stream] > 3,
            LandTypes.TerraceLand => counts[LandTypes.Hill] > 3,
            _ => false
        };
        if (!canBuild)
            return [];
        return points.Select(p => new SourceLand(p.Key, p.Value, targetType)).ToList();
    }

    private bool GetTerrains(Coordinate site, out Dictionary<LandTypes, int> counts, out Dictionary<Coordinate, Directions> points)
    {
        points = [];
        counts = new()
        {
            [LandTypes.Plain] = 0,
            [LandTypes.Stream] = 0,
            [LandTypes.Wood] = 0,
            [LandTypes.Hill] = 0,
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

    public List<Land> GetRoundLands(Coordinate site)
    {
        var lands = new List<Land>();
        var left = site.X - 2;
        var top = site.Y - 2;
        for (var i = 0; i < 5; i++)
        {
            for (var j = 0; j < 5; i++)
            {
                var point = SetPointWithin(new(left + i, top + j));
                lands.Add(this[point]);
            }
        }
        return lands;
    }

    public Coordinate SetPointWithin(Coordinate point)
    {
        var x = point.X % WorldWidth;
        if (x < 0)
            x += WorldWidth;
        var y = point.Y % WorldHeight;
        if (y < 0)
            y += WorldHeight;
        return new(x, y);
    }

    public static Coordinate SetPointWithin(Coordinate point, int worldWidth, int worldHeight)
    {
        var x = point.X % worldWidth;
        if (x < 0)
            x += worldWidth;
        var y = point.Y % worldHeight;
        if (y < 0)
            y += worldHeight;
        return new(x, y);
    }

    public Bitmap GetThumbnail()
    {
        var thumbnail = new Bitmap(WorldWidth, WorldHeight);
        var pThumbnail = new PointBitmap(thumbnail);
        pThumbnail.LockBits();
        for (int i = 0; i < WorldWidth; i++)
        {
            for (int j = 0; j < WorldHeight; j++)
            {
                var color = this[new(i, j)].Color;
                pThumbnail.SetPixel(i, j, color);
            }
        }
        pThumbnail.UnlockBits();
        return thumbnail;
    }

    public static List<LandPoint> ConvertLandPoints(AltitudeMap altitudeMap)
    {
        var landPoints = new Dictionary<Coordinate, LandPoint>();
        foreach (var (coordinate, point) in altitudeMap.AltitudePoints)
        {
            var site = SetPointWithin(coordinate, altitudeMap.Width, altitudeMap.Height);
            landPoints[site] = new(site, point.Altitude / altitudeMap.AltitudeMax, PointTypes.Normal);
        }
        foreach (var coordinate in altitudeMap.RiverPoints)
        {
            var site = SetPointWithin(coordinate, altitudeMap.Width, altitudeMap.Height);
            if (landPoints.TryGetValue(site, out var point))
                landPoints[site] = new(site, point.AltitudeRatio, PointTypes.River);
            else
                landPoints[site] = new(site, 0d, PointTypes.River);
        }
        foreach (var coordinate in altitudeMap.OriginPoints)
        {
            var site = SetPointWithin(coordinate, altitudeMap.Width, altitudeMap.Height);
            if (landPoints.TryGetValue(site, out var point))
                landPoints[site] = new(site, point.AltitudeRatio, PointTypes.Origin);
            else
                landPoints[site] = new(site, 1d, PointTypes.Origin);
        }
        return landPoints.Values.ToList();
    }
}
