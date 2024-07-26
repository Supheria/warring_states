using AltitudeMapGenerator;
using LocalUtilities.TypeGeneral;
using WarringStates.Map;
using WarringStates.User;

namespace WarringStates.Client.Map;

public static class Atlas
{
    static LandRoster<SingleLand> LandMap { get; set; } = [];

    public static Size WorldSize { get; private set; } = new();

    public static int WorldWidth => WorldSize.Width;

    public static int WorldHeight => WorldSize.Height;

    public static Land GetLand(this Coordinate coordinate)
    {
        if (LandMap.TryGetValue(coordinate, out var land))
            return land;
        return new SingleLand(coordinate, LandTypes.Plain);
    }

    //public static string GetLandTypeCount(this Enum type)
    //{
    //    return LandMap.GetLandTypeCount(type);
    //}

    public static void Relocate(PlayerArchive playerArchive)
    {
        LandMap.RosterList = playerArchive.VisibleLands;
        WorldSize = playerArchive.WorldSize;
        // TODO: broadcast event
    }

    public static Coordinate SetPointWithin(this Coordinate Point)
    {
        if (WorldWidth is 0 || WorldHeight is 0)
            return new();
        var x = Point.X % WorldWidth;
        if (x < 0)
            x += WorldWidth;
        var y = Point.Y % WorldHeight;
        if (y < 0)
            y += WorldHeight;
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
