using LocalUtilities.TypeGeneral;
using WarringStates.Client.Events;

namespace WarringStates.Client.UI.Component;

public partial class GamePlane : Displayer
{
    //double GridPaddingFactor { get; set; } = 0.02;

    public GamePlane()
    {
        AddOperations();
    }

    public void EnableListener()
    {
        LocalEvents.TryAddListener<Rectangle>(LocalEvents.UserInterface.InfoBarOnSetBounds, SetBounds);
        LocalEvents.TryAddListener(LocalEvents.Graph.GridOriginSet, Relocate);
    }

    public void DisableListener()
    {
        LocalEvents.TryRemoveListener<Rectangle>(LocalEvents.UserInterface.InfoBarOnSetBounds, SetBounds);
        LocalEvents.TryRemoveListener(LocalEvents.Graph.GridOriginSet, Relocate);
    }

    private void SetBounds(Rectangle rect)
    {
        Bounds = rect;
        base.Relocate();
        Relocate();
    }

    private new void Relocate()
    {
        LocalEvents.TryBroadcast(LocalEvents.Graph.GridToRelocate, new GridToRelocateArgs(Image, BackColor));
        Invalidate();
    }
}
