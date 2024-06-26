﻿using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Graph;
using LocalUtilities.TypeToolKit.Mathematic;
using System.Drawing.Drawing2D;
using WarringStates.Map.Terrain;
using WarringStates.User;

namespace WarringStates.Map;

public static class Atlas
{
    static LandMap LandMap { get; } = new();

    public static int Width => LandMap.Width;

    public static int Height => LandMap.Height;

    public static Size Size => LandMap.Size;

    public static ILand GetLand(this Coordinate coordinate)
    {
        return LandMap[coordinate];
    }

    public static string GetLandTypeCount(this Enum type)
    {
        return LandMap.GetLandTypeCount(type);
    }

    public static void Relocate(Archive archive)
    {
        LandMap.Relocate(archive.AltitudeMap, archive.SourceLands);
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

    //public static Bitmap GetOverview(Size size)
    //{
    //    var widthUnit = (size.Width / (double)Width).ToRoundInt();
    //    if (widthUnit is 0)
    //        widthUnit = 1;
    //    var heightUnit = (size.Height / (double)Height).ToRoundInt();
    //    if (heightUnit is 0)
    //        heightUnit = 1;
    //    var overview = new Bitmap(Width * widthUnit, Height * heightUnit);
    //    var pOverview = new PointBitmap(overview);
    //    pOverview.LockBits();
    //    for (int i = 0; i < Width; i++)
    //    {
    //        for (int j = 0; j < Height; j++)
    //        {
    //            var color = GetLand(new(i, j)).Color;
    //            drawUnit(i, j, color);
    //        }
    //    }
    //    pOverview.UnlockBits();
    //    return overview;
    //    void drawUnit(int col, int row, Color color)
    //    {
    //        var dx = widthUnit * col;
    //        var dy = heightUnit * row;
    //        for (var x = 0; x < widthUnit; x++)
    //        {
    //            for (var y = 0; y < heightUnit; y++)
    //            {
    //                pOverview.SetPixel(x + dx, y + dy, color);
    //            }
    //        }
    //    }
    //}
}
