using AtlasGenerator;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using System.Text;

namespace WarringStates.Map;

using Type = Terrain.Type;

internal class TerrainMap()
{
    internal Size Size { get; set; }

    Dictionary<Coordinate, Type> TerrainPoints { get; set; } = [];

    Dictionary<Type, int> TerrainCount { get; } = [];

    RandomTable Random { get; set; } = new();

    internal Type this[Coordinate coordinate]
    {
        get
        {
            if (TerrainPoints.TryGetValue(coordinate, out var terrain))
                return terrain;
            return Type.Plain;
        }
    }

    internal string GetTerrainCount(Type type)
    {
        var total = TerrainCount.Values.Sum();
        if (TerrainCount.TryGetValue(type, out var count))
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

    internal void Relocate(Atlas atlas)
    {
        TerrainPoints.Clear();
        Size = atlas.Bounds.Size;
        Random = new(atlas.RandomTable);
        foreach (var point in atlas.AltitudePoints)
        {
            var terrain = AltitudeFilter(point.Altitude / atlas.AltitudeMax);
            TerrainPoints[point] = terrain;
            if (TerrainCount.ContainsKey(terrain))
                TerrainCount[terrain]++;
            else
                TerrainCount[terrain] = 1;
        }
        foreach (var point in atlas.RiverPoints)
        {
            if (Random.Next().ApproxLessThan(0.25))
                continue;
            if (TerrainPoints.TryGetValue(point, out var terrain))
                TerrainCount[terrain]--;
            TerrainPoints[point] = Type.Stream;
            TerrainCount[Type.Stream]++;
        }
        TerrainCount[Type.Plain] = atlas.Area - TerrainCount.Sum(x => x.Key is Type.Plain ? 0 : x.Value);
    }

    private Type AltitudeFilter(double altitudeRatio)
    {
        var vote = Random.Next();
        if (altitudeRatio.ApproxLessThan(0.05))
        {
            if (vote.ApproxLessThan(0.33))
                return Type.Plain;
            if (vote.ApproxLessThan(0.9))
                return Type.Woodland;
        }
        else if (altitudeRatio.ApproxLessThan(0.15))
        {
            if (vote.ApproxLessThan(0.33))
                return Type.Woodland;
            if (vote.ApproxLessThan(0.95))
                return Type.Hill;
        }
        else
        {
            if (vote.ApproxLessThan(0.8))
                return Type.Hill;
            if (vote.ApproxLessThan(0.99))
                return Type.Woodland;
        }
        return Type.Stream;
    }
}
