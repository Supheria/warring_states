using LocalUtilities.TypeGeneral;
using WarringStates.Client.Events;
using WarringStates.Client.Graph;

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
        LocalEvents.TryAddListener(LocalEvents.Graph.GridOriginSet, GridRedrawAsync);
    }

    public void DisableListener()
    {
        LocalEvents.TryRemoveListener(LocalEvents.Graph.GridOriginSet, GridRedrawAsync);
    }

    public override void Redraw()
    {
        base.Redraw();
        GridRedrawAsync();
    }

    private async void GridRedrawAsync()
    {
        var source = await GridDrawer.RedrawAsync(ClientWidth, ClientHeight, BackColor);
        if (source is null)
            return;
        Image?.Dispose();
        Image = source;
    }
}
