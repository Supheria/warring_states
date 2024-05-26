using LocalUtilities.TypeGeneral;
using WarringStates.Events;
using WarringStates.Map;

namespace WarringStates.UI.Component;

public class InfoBrandDisplayer : Displayer
{
    SolidBrush InfoBrush { get; } = new(Color.White);

    public InfoBrandDisplayer()
    {
        LocalEvents.Hub.AddListener<GameDisplayerUpdateArgs>(LocalEvents.UserInterface.GameDisplayerUpdate, SetBounds);
    }

    private void SetBounds(GameDisplayerUpdateArgs args)
    {
        Bounds = args.OtherRect;
        Relocate();
        using var g = Graphics.FromImage(Image);
        g.Clear(Color.Gray);
        var info = $"\n水源{Terrain.Type.Stream.GetCount()}\n平原{Terrain.Type.Plain.GetCount()}\n树林{Terrain.Type.Woodland.GetCount()}\n山地{Terrain.Type.Hill.GetCount()}";
        g.DrawString(info, ContentFontData, InfoBrush, new Rectangle(new(0, 0), Size));
        Invalidate();
    }
}
