﻿using AltitudeMapGenerator;
using LocalUtilities.TypeGeneral;

namespace WarringStates.Map;

public static class Atlas
{
    static LandMap LandMap { get; } = new();

    public static int Width => LandMap.Size.Width;

    public static int Height => LandMap.Size.Height;

    public static Size Size => LandMap.Size;

    public static ILand GetLand(this Coordinate coordinate)
    {
        return LandMap[coordinate];
    }

    public static string GetLandTypeCount(this Enum type)
    {
        return LandMap.GetLandTypeCount(type);
    }

    public static void SetTerrainMap(this AltitudeMap atlas, List<SourceLand> sourceLands)
    {
        LandMap.Relocate(atlas, sourceLands);
    }

    public static Coordinate SetPointWithinTerrainMap(this Coordinate Point)
    {
        if (Width is 0 || Height is 0)
            return new();
        var x = Point.X % Width;
        if (x < 0)
            x += Width;
        var y = Point.Y % Height;
        if (y < 0)
            y += Height;
        return new(x, y);
    }
}