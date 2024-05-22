using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Graph;
using System.Drawing;

namespace WarringStates;

public partial class GameDisplayer : Displayer
{
    public GameDisplayer()
    {
        OnRelocate += Relocate;
        MouseDown += OnMouseDown;
        MouseMove += OnMouseMove;
        MouseUp += OnMouseUp;
    }

    protected override bool OnSetRange(Size range)
    {
        Size = range;
        return true;
    }

    protected override void Relocate()
    {
        var g = Graphics.FromImage(Image);
        g.Clear(Color.Transparent);
        Image.DrawLatticeGrid();
        var infoRect = new Rectangle(0, Image.Height - 100, Image.Width, 100);
        g.FillRectangle(new SolidBrush(Color.Gray), infoRect);
        g.DrawString($"\n水源{Terrain.Type.Stream.GetCount()}\n平原{Terrain.Type.Plain.GetCount()}\n树林{Terrain.Type.Woodland.GetCount()}\n山地{Terrain.Type.Hill.GetCount()}",
            new("仿宋", 15, FontStyle.Bold, GraphicsUnit.Pixel), new SolidBrush(Color.White), infoRect);
        Image.Save("_GameDisplayer.bmp");
        Invalidate();
    }
}
