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
        LocalEvents.Hub.AddListener(LocalEvents.Graph.GridOriginReset, UpdateImage);
    }

    private void SetBounds(Rectangle rect)
    {
        Bounds = rect;
        Relocate();
        LocalEvents.Hub.Broadcast(LocalEvents.Graph.GridImageToUpdate, new GridImageToUpdateArgs(Image, BackColor));
        Invalidate();
    }

    private void Relocate(int dX, int dY)
    {
        Relocate();
        LocalEvents.Hub.Broadcast(LocalEvents.Graph.OffsetGridOrigin, new Coordinate(dX, dY));
    }

    private void UpdateImage()
    {
        LocalEvents.Hub.Broadcast(LocalEvents.Graph.GridImageToUpdate, new GridImageToUpdateArgs(Image, BackColor));
        Invalidate();
    }
}
