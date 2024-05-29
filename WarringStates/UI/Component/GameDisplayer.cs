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
        LocalEvents.Hub.AddListener<GameFormUpdatedArgs>(LocalEvents.UserInterface.GameFormUpdate, SetBounds);
        LocalEvents.Hub.AddListener(LocalEvents.Graph.GridOriginReset, UpdateImage);
    }

    private void SetBounds(GameFormUpdatedArgs args)
    {
        Location = args.GameRect.Location;
        Size = new(args.GameRect.Width, args.GameRect.Height - InfoBrandHeight);
        Relocate();
        var otherRect = new Rectangle(Left, Bottom, Width, args.GameRect.Height - Height);
        LocalEvents.Hub.Broadcast(LocalEvents.UserInterface.GameDisplayerUpdate, new GameDisplayerUpdatedArgs(Bounds, otherRect));
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
