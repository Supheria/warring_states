using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using WarringStates.Client.Events;
using WarringStates.Map;
using WarringStates.User;

namespace WarringStates.Client.Map;

public static class Atlas
{
    static LandMapEx LandMap { get; set; } = new();

    public static Size Size => LandMap.WorldSize;

    public static int Width => LandMap.WorldWidth;

    public static int Height => LandMap.WorldHeight;

    public static Land GetLand(this Coordinate coordinate)
    {
        return LandMap[coordinate];
    }

    //public static string GetLandTypeCount(this Enum type)
    //{
    //    return LandMap.GetLandTypeCount(type);
    //}

    public static void Relocate(PlayerArchive playerArchive)
    {
        LandMap.Relocate(playerArchive.VisibleLands, playerArchive.WorldSize);
        // TODO: broadcast event
    }

    public static Coordinate SetPointWithin(Coordinate Point)
    {
        return LandMap.SetPointWithin(Point);
    }

    public static void AddVision(VisibleLands vision)
    {
        LandMap.AddVision(vision);
        LocalEvents.TryBroadcast(LocalEvents.Map.AtlasUpdate);
    }

    public static Bitmap GetOverview(Size size)
    {
        var widthUnit = (size.Width / (double)Width).ToRoundInt();
        if (widthUnit is 0)
            widthUnit = 1;
        var heightUnit = (size.Height / (double)Height).ToRoundInt();
        if (heightUnit is 0)
            heightUnit = 1;
        var overview = new Bitmap(Width * widthUnit, Height * heightUnit);
        var pOverview = new PointBitmap(overview);
        pOverview.LockBits();
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                var color = GetLand(new(i, j)).Color;
                drawUnit(i, j, color);
            }
        }
        pOverview.UnlockBits();
        return overview;
        void drawUnit(int col, int row, Color color)
        {
            var dx = widthUnit * col;
            var dy = heightUnit * row;
            for (var x = 0; x < widthUnit; x++)
            {
                for (var y = 0; y < heightUnit; y++)
                {
                    pOverview.SetPixel(x + dx, y + dy, color);
                }
            }
        }
    }
}
