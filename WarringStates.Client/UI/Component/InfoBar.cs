using LocalUtilities.TypeGeneral;
using WarringStates.Client.Events;
using WarringStates.Client.Map;
using WarringStates.Map.Terrain;

namespace WarringStates.Client.UI.Component;

public class InfoBar : Displayer
{
    SolidBrush InfoBrush { get; } = new(Color.White);

    public InfoBar()
    {
        Height = 100;
    }

    public void EnableListener()
    {
        LocalEvents.TryAddListener<Rectangle>(LocalEvents.UserInterface.ToolBarOnSetBounds, SetBounds);
    }

    public void DisableListener()
    {
        LocalEvents.TryRemoveListener<Rectangle>(LocalEvents.UserInterface.ToolBarOnSetBounds, SetBounds);
    }


    private void SetBounds(Rectangle rect)
    {
        Bounds = new(rect.Left, rect.Bottom - Height, rect.Width, Height);
        Relocate();
        using var g = Graphics.FromImage(Image);
        g.Clear(Color.Gray);
        var info = $"\n水源{SingleLand.Types.Stream.GetLandTypeCount()}\n平原{SingleLand.Types.Plain.GetLandTypeCount()}\n树林{SingleLand.Types.Wood.GetLandTypeCount()}\n山地{SingleLand.Types.Hill.GetLandTypeCount()}";
        g.DrawString(info, ContentFontData, InfoBrush, new Rectangle(new(0, 0), Size));
        Invalidate();
        rect = new(rect.Left, rect.Top, rect.Width, rect.Height - Height);
        LocalEvents.TryBroadcast(LocalEvents.UserInterface.InfoBarOnSetBounds, rect);
    }
}
