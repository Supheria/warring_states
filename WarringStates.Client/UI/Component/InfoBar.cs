using LocalUtilities.TypeGeneral;
using WarringStates.Client.Map;
using WarringStates.Map.Terrain;

namespace WarringStates.Client.UI.Component;

public class InfoBar : Displayer
{
    SolidBrush InfoBrush { get; } = new(Color.White);

    public override void Redraw()
    {
        base.Redraw();
        using var g = Graphics.FromImage(Image);
        g.Clear(Color.Gray);
        var info = $"\n水源{SingleLand.Types.Stream.GetLandTypeCount()}\n平原{SingleLand.Types.Plain.GetLandTypeCount()}\n树林{SingleLand.Types.Wood.GetLandTypeCount()}\n山地{SingleLand.Types.Hill.GetLandTypeCount()}";
        g.DrawString(info, ContentFontData, InfoBrush, ClientRect);
    }
}
