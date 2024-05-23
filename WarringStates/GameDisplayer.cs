using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.EventProcess;

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

    private Rectangle GetGridRect()
    {
        var padding = new Size((int)(Width * GridPaddingFactor), (int)(Height * GridPaddingFactor));
        return new Rectangle(padding.Width, padding.Height, Width - 2 * padding.Width, Height - InfoBrandHeight - 2 * padding.Height);
    }

    protected override bool OnSetRange(Size range)
    {
        Size = range;
        return true;
    }

    protected override void Relocate()
    {
        Relocate(0, 0);
    }

    private void Relocate(int dX, int dY)
    {
        EventManager.Instance.Dispatch(LocalEventId.ImageUpdate, new GridToUpdateEventArgument(Image, GetGridRect(), BackColor, new(dX, dY)));
        DrawInfoBrand();
        Invalidate();
    }

    private void DrawInfoBrand()
    {
        var g = Graphics.FromImage(Image);
        var infoRect = new Rectangle(0, Height - InfoBrandHeight, Width, InfoBrandHeight);
        g.FillRectangle(new SolidBrush(Color.Gray), infoRect);
        g.DrawString($"\n水源{Terrain.Type.Stream.GetCount()}\n平原{Terrain.Type.Plain.GetCount()}\n树林{Terrain.Type.Woodland.GetCount()}\n山地{Terrain.Type.Hill.GetCount()}",
            new("仿宋", 15, FontStyle.Bold, GraphicsUnit.Pixel), new SolidBrush(Color.White), infoRect);
        //Image.Save("_GameDisplayer.bmp");
    }
}
