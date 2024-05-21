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

    public class TestPair(double random, double ratio, Type type) : ISsSerializable
    {
        public string LocalName { get; set; } = nameof(TestPair);

        double Random { get; set; } = random;

        double Ratio { get; set; } = ratio;

        Type Type { get; set; } = type;

        public TestPair() : this(0, 0, Type.Plain) { }

        public void Serialize(SsSerializer serializer)
        {
            serializer.WriteTag(nameof(Random), Random.ToString());
            serializer.WriteTag(nameof(Ratio), Ratio.ToString());
            serializer.WriteTag(nameof(Type), Type.ToString());
        }

        public void Deserialize(SsDeserializer deserializer)
        {
            
        }
    }

    internal void Relocate(Atlas atlas)
    {
        TerrainPoints.Clear();
        Size = atlas.Bounds.Size;
        Random = new(atlas.RandomTable);
        var testPairs = new List<TestPair>();
        foreach (var point in atlas.AltitudePoints)
        {
            var ratio = point.Altitude / atlas.AltitudeMax;
            var terrain = AltitudeFilter(ratio);
            testPairs.Add(new(Random.Current(), ratio, terrain));
            TerrainPoints[point] = terrain;
            addCount(terrain);
        }
        testPairs.SaveToSimpleScript("testPair", true, "testpair.ss");
        foreach (var point in atlas.RiverPoints)
        {
            //var vote = Random.Next();
            if (TerrainPoints.ContainsKey(point))
                continue;
            TerrainPoints[point] = Type.Stream;
            addCount(Type.Stream);
        }
        TerrainCount[Type.Plain] += atlas.Area - TerrainCount.Sum(x => x.Key is Type.Plain ? 0 : x.Value);
        void addCount(Type terrain)
        {
            if (TerrainCount.ContainsKey(terrain))
                TerrainCount[terrain]++;
            else
                TerrainCount[terrain] = 1;
        }
    }

    private Type AltitudeFilter(double altitudeRatio)
    {
        var vote = Random.Next();
        if (altitudeRatio.ApproxLessThan(0.05))
        {
            if (vote.ApproxLessThan(0.5))
                return Type.Plain;
            if (vote.ApproxLessThan(0.95))
                return Type.Woodland;
        }
        else if (altitudeRatio.ApproxLessThan(0.15))
        {
            if (vote.ApproxLessThan(0.5))
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
