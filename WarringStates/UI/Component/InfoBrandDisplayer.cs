using LocalUtilities.TypeGeneral;
using WarringStates.Events;
using WarringStates.Map;

namespace WarringStates.UI.Component;

public class InfoBrandDisplayer : Displayer
{
    SolidBrush InfoBrush { get; } = new(Color.White);

    public InfoBrandDisplayer()
    {
        LocalEvents.Hub.AddListener<GameDisplayerUpdatedArgs>(LocalEvents.UserInterface.GameDisplayerUpdate, SetBounds);
    }

    private void SetBounds(GameDisplayerUpdatedArgs args)
    {
        Bounds = args.OtherRect;
        Relocate();
        using var g = Graphics.FromImage(Image);
        g.Clear(Color.Gray);
        var info = $"\n水源{SingleLand.Types.Stream.GetLandTypeCount()}\n平原{SingleLand.Types.Plain.GetLandTypeCount()}\n树林{SingleLand.Types.Wood.GetLandTypeCount()}\n山地{SingleLand.Types.Hill.GetLandTypeCount()}";
        g.DrawString(info, ContentFontData, InfoBrush, new Rectangle(new(0, 0), Size));
        Invalidate();
    }
}
