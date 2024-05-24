using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.EventProcess;

namespace WarringStates;

public partial class GameDisplayer : Displayer, IEventListener
{
    double GridPaddingFactor { get; set; } = 0.02;

    int InfoBrandHeight { get; set; } = 100;

    public GameDisplayer()
    {
        MouseDown += OnMouseDown;
        MouseMove += OnMouseMove;
        MouseUp += OnMouseUp;
    }
    public void EnableListener()
    {
        EventManager.Instance.AddEvent(LocalEventId.GameFormUpdate, this);
    }

    public void HandleEvent(int eventId, IEventArgument argument)
    {
        if (eventId is LocalEventId.GameFormUpdate)
        {
            if (argument is not GameFormUpdateEventArgument arg)
                return;
            Size = arg.ClientSize;
            Relocate(0, 0);
        }
    }

    private Rectangle GetGridRect()
    {
        var padding = new Size((int)(Width * GridPaddingFactor), (int)(Height * GridPaddingFactor));
        return new Rectangle(padding.Width, padding.Height, Width - 2 * padding.Width, Height - InfoBrandHeight - 2 * padding.Height);
    }

    private void Relocate(int dX, int dY)
    {
        Relocate();
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
