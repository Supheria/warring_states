using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using WarringStates.Client.Events;
using WarringStates.Map;

namespace WarringStates.Client.Map;

public class AtlasEx : Atlas
{
    public static Land GetLand(Coordinate point)
    {
        if (SourceLands.TryGetValue(point, out var sourceLand))
            return sourceLand;
        if (SingleLands.TryGetValue(point, out var singleLand))
            return singleLand;
        return new SingleLand(point, SingleLandTypes.None);
    }

    public static void Relocate(AtlasData playerArchive)
    {
        SingleLands.Clear();
        SingleLands.AddArange(playerArchive.VisibleLands.SingleLands);
        SourceLands.Clear();
        SourceLands.AddArange(playerArchive.VisibleLands.SourceLands);
        Size = playerArchive.WorldSize;
    }

    public static void AddVision(VisibleLands vision)
    {
        SingleLands.AddArange(vision.SingleLands);
        SourceLands.AddArange(vision.SourceLands);
        LocalEvents.TryBroadcast(LocalEvents.Map.AtlasUpdate);
    }

    public static Bitmap? GetOverview(Size size)
    {
        if (Width is 0 || Height is 0)
            return null;
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
