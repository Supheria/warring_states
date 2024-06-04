using LocalUtilities.TypeGeneral;
using WarringStates.Events;

namespace WarringStates.UI.Component;

public partial class GamePlane : Displayer
{
    //double GridPaddingFactor { get; set; } = 0.02;

    public GamePlane()
    {
        AddOperations();
        LocalEvents.Hub.AddListener<Rectangle>(LocalEvents.UserInterface.InfoBarOnSetBounds, SetBounds);
        LocalEvents.Hub.AddListener(LocalEvents.Graph.GridOriginSet, Relocate);
    }

    private void SetBounds(Rectangle rect)
    {
        Bounds = rect;
        base.Relocate();
        Relocate();
    }

    private new void Relocate()
    {
        LocalEvents.Hub.Broadcast(LocalEvents.Graph.GridToRelocate, new GridToRelocateArgs(Image, BackColor));
        Invalidate();
    }
}
