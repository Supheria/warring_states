using LocalUtilities.TypeGeneral;
using WarringStates.Events;

namespace WarringStates.UI.Component;

public partial class GamePlane : Displayer
{
    //double GridPaddingFactor { get; set; } = 0.02;

    public GamePlane()
    {
        AddOperations();
    }

    public void EnableListener()
    {
        LocalEvents.Hub.TryAddListener<Rectangle>(LocalEvents.UserInterface.InfoBarOnSetBounds, SetBounds);
        LocalEvents.Hub.TryAddListener(LocalEvents.Graph.GridOriginSet, Relocate);
    }

    public void DisableListener()
    {
        LocalEvents.Hub.TryRemoveListener<Rectangle>(LocalEvents.UserInterface.InfoBarOnSetBounds, SetBounds);
        LocalEvents.Hub.TryRemoveListener(LocalEvents.Graph.GridOriginSet, Relocate);
    }

    private void SetBounds(Rectangle rect)
    {
        Bounds = rect;
        base.Relocate();
        Relocate();
    }

    private new void Relocate()
    {
        LocalEvents.Hub.TryBroadcast(LocalEvents.Graph.GridToRelocate, new GridToRelocateArgs(Image, BackColor));
        Invalidate();
    }
}
