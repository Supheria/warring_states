using AtlasGenerator;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeGeneral.Convert;
using LocalUtilities.TypeToolKit.Mathematic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarringStates;

using Type = Terrain.Type;

internal class TerrainMap()
{
    internal Size Size { get; set; }

    internal int Width => Size.Width;

    internal int Height => Size.Height;

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
        var sb = new StringBuilder();
        if (type is Type.Total)
            sb.Append(total);
        else if (TerrainCount.TryGetValue(type, out var count))
            sb.Append(Math.Round(count / (double)total * 100, 2));
        else
            sb.Append(0);
        return sb.Append('%')
            .ToString();
    }

    internal void Relocate(Atlas atlas)
    {
        TerrainPoints.Clear();
        Size = atlas.Bounds.Size;
        Random = new(atlas.RandomTable);
        var count = 0;
        foreach (var point in atlas.AltitudePoints)
        {
            var terrain = AltitudeFilter(point.Altitude / atlas.AltitudeMax);
            TerrainPoints[point] = terrain;
            addCount(terrain);
        }
        foreach (var point in atlas.RiverPoints)
        {
            var vote = Random.Next();
            if (vote < 0.75)
            {
                TerrainPoints[point] = Type.Stream;
                addCount(Type.Stream);
            }
        }
        TerrainCount[Type.Plain] = atlas.Area - count;
        void addCount(Type terrain)
        {
            if (TerrainCount.ContainsKey(terrain))
                TerrainCount[terrain]++;
            else
                TerrainCount[terrain] = 1;
            count++;
        }
    }

    private Type AltitudeFilter(double altitudeRatio)
    {
        var vote = Random.Next();
        if (altitudeRatio.ApproxLessThan(0.05))
        {
            if (vote < 0.5)
                return Type.Plain;
            if (vote < 0.95)
                return Type.Woodland;
            return Type.Stream;
        }
        if (altitudeRatio.ApproxLessThan(0.15))
        {
            if (vote < 0.5)
                return Type.Woodland;
            if (vote < 0.95)
                return Type.Hill;
            return Type.Stream;
        }
        if (vote < 0.8)
            return Type.Hill;
        if (vote < 0.99)
            return Type.Woodland;
        return Type.Stream;
    }
}
