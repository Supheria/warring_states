using LocalUtilities.TypeGeneral;

namespace WarringStates;

internal class GameDisplayer : PictureBox
{
    public void ResetImage()
    {
        Image?.Dispose();
        Image = new Bitmap(Width, Height);
        var g = Graphics.FromImage(Image);
        g.Clear(Color.LightYellow);
        var pImage = new PointBitmap(Image);
        pImage.LockBits();
        for (int i = 0; i < Image.Width; i++)
        {
            for (int j = 0; j < Image.Height; j++)
            {
                if (Terrain.TerrainMap.TryGetValue(new(i, j), out var terrain))
                {
                    switch (terrain)
                    {
                        case Terrain.Type.Stream:
                            pImage.SetPixel(i, j, Color.SkyBlue);
                            continue;
                        case Terrain.Type.Woodland:
                            pImage.SetPixel(i, j, Color.ForestGreen);
                            continue;
                        case Terrain.Type.Hill:
                            pImage.SetPixel(i, j, Color.Black);
                            continue;
                    }
                }
            }
        }
        pImage.UnlockBits();
        Image.Save("map.bmp");
    }
}
