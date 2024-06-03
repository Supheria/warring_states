using LocalUtilities.TypeGeneral;
using WarringStates.Events;

namespace WarringStates.UI.Component;

public partial class GameDisplayer : Displayer
{
    //double GridPaddingFactor { get; set; } = 0.02;

    int InfoBrandHeight { get; set; } = 100;

    public GameDisplayer()
    {
        AddOperations();
    }

    public void EnableListener()
    {
        LocalEvents.Hub.AddListener<Rectangle>(LocalEvents.UserInterface.ToolBrandDisplayerOnResize, SetBounds);
        LocalEvents.Hub.AddListener(LocalEvents.Graph.GridOriginReset, UpdateImage);
    }

    private void SetBounds(Rectangle rect)
    {
        Location = rect.Location;
        Size = new( Width, rect.Height - InfoBrandHeight);
        Relocate();
        rect = new Rectangle(Left, Bottom, Width, rect.Height - Height);
        LocalEvents.Hub.Broadcast(LocalEvents.UserInterface.GameDisplayerOnResize, new GameDisplayerUpdatedArgs(Bounds, rect));
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
