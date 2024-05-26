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
        LocalEvents.Hub.AddListener<GameFormUpdateArgs>(LocalEvents.UserInterface.GameFormUpdate, SetBounds);
    }

    private void SetBounds(GameFormUpdateArgs args)
    {
        Location = args.GameRect.Location;
        Size = new(args.GameRect.Width, args.GameRect.Height - InfoBrandHeight);
        Relocate();
        var otherRect = new Rectangle(Left, Bottom, Width, args.GameRect.Height - Height);
        LocalEvents.Hub.Broadcast(LocalEvents.UserInterface.GameDisplayerUpdate, new GameDisplayerUpdateArgs(Bounds, otherRect));
        LocalEvents.Hub.Broadcast(LocalEvents.Graph.GameImageUpdate, new GameImageUpdateArgs(Image, BackColor, new(0, 0)));
        Invalidate();
    }

    private void Relocate(int dX, int dY)
    {
        Relocate();
        LocalEvents.Hub.Broadcast(LocalEvents.Graph.GameImageUpdate, new GameImageUpdateArgs(Image, BackColor, new(dX, dY)));
        Invalidate();
    }
}
