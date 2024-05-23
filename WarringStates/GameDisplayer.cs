using LocalUtilities.TypeGeneral;

namespace WarringStates;

public partial class GameDisplayer : Displayer
{
    double GridPaddingFactor { get; set; } = 0.02;

    int InfoBrandHeight { get; set; } = 100;

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
        //g.Clear(Color.Transparent);
        var padding = new Size((int)(Width * GridPaddingFactor), (int)(Height * GridPaddingFactor));
        var gridRect = new Rectangle(padding.Width, padding.Height, Width - 2 * padding.Width, Height - InfoBrandHeight - 2 * padding.Height);
        Image.DrawLatticeGrid(gridRect, BackColor);
        var infoRect = new Rectangle(0, Height - InfoBrandHeight, Width, InfoBrandHeight);
        g.FillRectangle(new SolidBrush(Color.Gray), infoRect);
        g.DrawString($"\n水源{Terrain.Type.Stream.GetCount()}\n平原{Terrain.Type.Plain.GetCount()}\n树林{Terrain.Type.Woodland.GetCount()}\n山地{Terrain.Type.Hill.GetCount()}",
            new("仿宋", 15, FontStyle.Bold, GraphicsUnit.Pixel), new SolidBrush(Color.White), infoRect);
        Image.Save("_GameDisplayer.bmp");
        Invalidate();
    }
}
