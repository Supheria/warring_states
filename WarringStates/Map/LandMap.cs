﻿using AltitudeMapGenerator;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using System.Text;
using WarringStates.Map.Terrain;
using WarringStates.Terrain;

namespace WarringStates.Map;

internal class LandMap()
{
    internal Size Size { get; set; }

    internal int Width => Size.Width;

    internal int Height => Size.Height;

    Dictionary<Coordinate, ILand> LandPoints { get; set; } = [];

    Dictionary<Enum, int> LandCount { get; } = [];

    internal ILand this[Coordinate point]
    {
        get
        {
            if (LandPoints.TryGetValue(point, out var terrain))
                return terrain;
            return new SingleLand(point, SingleLand.Types.Plain);
        }
    }

    internal string GetLandTypeCount(Enum type)
    {
        var total = LandCount.Values.Sum();
        if (LandCount.TryGetValue(type, out var count))
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

    internal void Relocate(AltitudeMap altitudeMap, List<SourceLand> sourceLands)
    {
        Relocate(altitudeMap);
        foreach (var sourceLand in sourceLands)
        {
            foreach (var point in sourceLand.GetPoints())
            {
                if (LandPoints.TryGetValue(point, out var land) && land is SingleLand singleLand)
                    LandCount[singleLand.Type]--;
                LandPoints[point] = sourceLand;
            }
            if (LandCount.ContainsKey(sourceLand.Type))
                LandCount[sourceLand.Type]++;
            else
                LandCount[sourceLand.Type] = 1;
        }
    }

    internal void Relocate(AltitudeMap altitudeMap)
    {
        LandPoints.Clear();
        LandCount.Clear();
        Size = altitudeMap.Bounds.Size;
        var random = new RandomTable(altitudeMap.RandomTable);
        foreach (var point in altitudeMap.AltitudePoints)
        {
            var singleLand = new SingleLand(point, point.Altitude / altitudeMap.AltitudeMax, random.Next());
            LandPoints[point] = singleLand;
            if (LandCount.ContainsKey(singleLand.Type))
                LandCount[singleLand.Type]++;
            else
                LandCount[singleLand.Type] = 1;
        }
        foreach (var point in altitudeMap.RiverPoints)
        {
            if (random.Next().ApproxLessThan(0.25))
                continue;
            if (LandPoints.TryGetValue(point, out var land) && land is SingleLand singleLand)
                LandCount[singleLand.Type]--;
            LandPoints[point] = new SingleLand(point, SingleLand.Types.Stream);
            LandCount[SingleLand.Types.Stream]++;
        }
        LandCount[SingleLand.Types.Plain] = altitudeMap.Area - LandCount.Sum(x => x.Key is SingleLand.Types.Plain ? 0 : x.Value);
    }
}
