using LocalUtilities.TypeGeneral;
using WarringStates.Client.Events;
using WarringStates.Client.Graph;

namespace WarringStates.Client.UI.Component;

public partial class GamePlane : Displayer
{
    public override void EnableListener()
    {
        base.EnableListener();
        LocalEvents.TryAddListener(LocalEvents.Graph.GridOriginReset, BeginDrawGrid);
        LocalEvents.TryAddListener<GridRedrawArgs>(LocalEvents.Graph.GridRedraw, EndDrawGrid);
        LocalEvents.TryAddListener<GridCellPointedOnArgs>(LocalEvents.Graph.PointOnCell, PointOnCell);
    }

    public override void DisableListener()
    {
        base.DisableListener();
        LocalEvents.TryRemoveListener(LocalEvents.Graph.GridOriginReset, BeginDrawGrid);
        LocalEvents.TryRemoveListener<GridRedrawArgs>(LocalEvents.Graph.GridRedraw, EndDrawGrid);
        LocalEvents.TryRemoveListener<GridCellPointedOnArgs>(LocalEvents.Graph.PointOnCell, PointOnCell);
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
