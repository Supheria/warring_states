using AtlasGenerator;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;

namespace WarringStates;

public static class Terrain
{
    public enum Type
    {
        Plain,
        Hill,
        Stream,
        Woodland,
    }

    public static TerrainOverviewColors OverviewColors { get; set; } = new TerrainOverviewColors().LoadFromSimpleScript();

    static TerrainMap TerrainMap { get; } = new();

    public static Bitmap? Overview { get; private set; }

    public static int Width => TerrainMap.Size.Width;

    public static int Height => TerrainMap.Size.Height;

    public static Type GetTerrain(this Coordinate coordinate)
    {
        return TerrainMap[coordinate];
    }

    public static string GetCount(this Type type)
    {
        return TerrainMap.GetTerrainCount(type);
    }

    public static void SetTerrainMap(this Atlas atlas)
    {
        TerrainMap.Relocate(atlas);
        DrawOverView();
    }

    public static void DrawOverView()
    {
        if (TerrainMap is null)
            return;
        Overview?.Dispose();
        Overview = new(Width, Height);
        var pOverview = new PointBitmap(Overview);
        pOverview.LockBits();
        for (int i = 0; i < Overview.Width; i++)
        {
            for (int j = 0; j < Overview.Height; j++)
                pOverview.SetPixel(i, j, OverviewColors[TerrainMap[new(i, j)]]);
        }
        pOverview.UnlockBits();
    }
}
