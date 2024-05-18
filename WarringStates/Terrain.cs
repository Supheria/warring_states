using AtlasGenerator;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;

namespace WarringStates;

internal class Terrain
{
    public enum Type
    {
        Plain,
        Hill,
        Stream,
        Woodland
    }

    public static Atlas Atlas
    {
        get => _atlas;
        set
        {
            _atlas = value;
            SetTerrainMap();
        }
    }
    private static Atlas _atlas = new Atlas().LoadFromSimpleScript();

    public static Dictionary<Coordinate, Type> TerrainMap
    {
        get
        {
            if (_terrainMap is null)
            {
                _terrainMap = [];
                SetTerrainMap();
            }
            return _terrainMap;
        }
    }
    static Dictionary<Coordinate, Type>? _terrainMap = null;

    private static void SetTerrainMap()
    {
        Atlas.RiverPoints.ForEach(p => TerrainMap[p] = Type.Stream);
        Atlas.AltitudePoints.ForEach(p => TerrainMap[p] = GetTerrainType(p.Altitude / Atlas.AltitudeMax));
    }

    public static Type GetTerrainType(double altitudeRatio)
    {
        if (altitudeRatio < 0 || altitudeRatio > 1)
            throw AtlasException.AltitudeRatioOutRange();
        if (altitudeRatio.ApproxLessThan(1d / 35d))
            return Type.Plain;
        if (altitudeRatio.ApproxLessThan(3d / 35d))
            return Type.Woodland;
        if (altitudeRatio.ApproxLessThan(5d / 35d))
            return Type.Stream;
        if (altitudeRatio.ApproxGreaterThan(20d / 35d))
            return Type.Stream;
        if (altitudeRatio.ApproxGreaterThan(15d / 35d))
            return Type.Woodland;
        return Type.Hill;
    }
}
