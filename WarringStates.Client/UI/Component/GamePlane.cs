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
        LocalEvents.TryAddListener(LocalEvents.Graph.GridOriginReset, BeginDrawGrid);
        LocalEvents.TryAddListener<GridRedrawArgs>(LocalEvents.Graph.GridRedraw, EndDrawGrid);
    }

    public void DisableListener()
    {
        LocalEvents.TryRemoveListener(LocalEvents.Graph.GridOriginReset, BeginDrawGrid);
        LocalEvents.TryRemoveListener<GridRedrawArgs>(LocalEvents.Graph.GridRedraw, EndDrawGrid);
    }

    public override void Redraw()
    {
        base.Redraw();
        BeginDrawGrid();
    }

    private void BeginDrawGrid()
    {
        GridDrawer.RedrawAsync(ClientWidth, ClientHeight, BackColor);
    }

    private void EndDrawGrid(GridRedrawArgs args)
    {
        Image?.Dispose();
        Image = args.Source;
        Update();
    }
}
