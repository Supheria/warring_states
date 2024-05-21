using AtlasGenerator;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Graph;
using System.DirectoryServices.ActiveDirectory;

namespace WarringStates;

internal class GameDisplayer : PictureBox
{
    public new Rectangle Bounds
    {
        get => base.Bounds;
        set
        {
            base.Bounds = value;
            ResetImage();
        }
    }

    Image? Cache { get; set; }

    Bitmap? OverView { get; set; } = null;

    Bitmap? OverViewLarge { get; set; } = null;

    private void ResetImage()
    {
        Image = Cache ?? Image;
        Image?.Dispose();
        Image = new Bitmap(Width, Height);
        DrawOverview();

        var g = Graphics.FromImage(Image);

        var infoRect = new Rectangle(0, Image.Height - 100, Image.Width, 100);
        g.FillRectangle(new SolidBrush(Color.Gray), infoRect);
        g.DrawString($"\n水源{Terrain.Type.Stream.GetCount()}\n平原{Terrain.Type.Plain.GetCount()}\n树林{Terrain.Type.Woodland.GetCount()}\n山地{Terrain.Type.Hill.GetCount()}",
            new("仿宋", 15, FontStyle.Bold, GraphicsUnit.Pixel), new SolidBrush(Color.White), infoRect);
        Image.Save("map.bmp");
    }

    private void DrawOverview()
    {
        var overViewSize = Terrain.Overview?.ScaleToSizeOnRatio(new((int)(Width * 0.33), (int)(Height * 0.33)));
        OverView = OverView is null 
            ? getScaledOverview(overViewSize) : OverView.Size != overViewSize 
            ? getScaledOverview(overViewSize) : OverView;
        if (OverView is not null) 
            Image.DrawTemplateIntoRect(OverView, new Rectangle(Right - OverView.Width - 20, Top + 20, OverView.Width, OverView.Width), true);
        overViewSize = Terrain.Overview?.ScaleToSizeOnRatio(new(Image.Width, Image.Height - 100));
        OverViewLarge = OverViewLarge is null
            ? getScaledOverview(overViewSize) : OverViewLarge.Size != overViewSize
            ? getScaledOverview(overViewSize) : OverViewLarge;
        if (OverViewLarge is not null)
            Image.DrawTemplateIntoRect(OverViewLarge, new Rectangle(Right - OverViewLarge.Width, Top, OverViewLarge.Width, OverViewLarge.Width), true);
        //Cache = Image;
        //if (OverViewLarge is not null)
        //    Image = OverViewLarge;
        static Bitmap? getScaledOverview(Size? size)
        {
            if (size is null || Terrain.Overview is null)
                return null;
            return Terrain.Overview.CopyToNewSize(size.Value);

        }
    }
}
