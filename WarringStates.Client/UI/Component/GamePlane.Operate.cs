//#define MOUSE_DRAG_FREE

using LocalUtilities.TypeGeneral;
using WarringStates.Client.Events;
using WarringStates.Client.Graph;
using WarringStates.Client.Map;
using WarringStates.Client.Net;
using WarringStates.Map;

namespace WarringStates.Client.UI.Component;

partial class GamePlane
{
    public delegate void DrawImageHandler();

    Coordinate PointOnCellSite { get; set; } = new();

    public static Color FocusColor { get; } = Color.Red;

    public static Color SelectColor { get; } = Color.Red;

    bool DoDragGraph { get; set; } = false;

    Point DragStartPoint { get; set; } = new();

    public DrawImageHandler? OnDragImage { get; set; }

    public override void EnableListener()
    {
        base.EnableListener();
        LocalEvents.TryAddListener(LocalEvents.Graph.GridReset, BeginDrawGrid);
        LocalEvents.TryAddListener<GridRedrawArgs>(LocalEvents.Graph.GridRedraw, EndDrawGrid);
        LocalEvents.TryAddListener<GridCellPointedOnArgs>(LocalEvents.Graph.PointOnCell, PointOnCell);
        LocalEvents.TryAddListener<SourceLandCanBuildArgs>(LocalEvents.UserInterface.SourceLandCanBuild, ShowCanBuildTypes);
        LocalEvents.TryAddListener(LocalEvents.Map.AtlasUpdate, BeginDrawGrid);
    }

    public override void DisableListener()
    {
        base.DisableListener();
        LocalEvents.TryRemoveListener(LocalEvents.Graph.GridReset, BeginDrawGrid);
        LocalEvents.TryRemoveListener<GridRedrawArgs>(LocalEvents.Graph.GridRedraw, EndDrawGrid);
        LocalEvents.TryRemoveListener<GridCellPointedOnArgs>(LocalEvents.Graph.PointOnCell, PointOnCell);
        LocalEvents.TryRemoveListener<SourceLandCanBuildArgs>(LocalEvents.UserInterface.SourceLandCanBuild, ShowCanBuildTypes);
        LocalEvents.TryRemoveListener(LocalEvents.Map.AtlasUpdate, BeginDrawGrid);
    }

    private void ShowCanBuildTypes(SourceLandCanBuildArgs args)
    {
        GridDrawer.DrawFocus(ClientSize, BackColor, args.Site);
    }

    private void PointOnCell(GridCellPointedOnArgs args)
    {
        if (args.MouseOperate is MouseOperates.LeftDoubleClick)
        {
            PointOnCellSite = args.Site;
            LocalNet.Service.CheckBuildLand(PointOnCellSite);
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button is MouseButtons.Left)
        {
            DoDragGraph = true;
            DragStartPoint = e.Location;
            GridDrawer.PointOnCell(e.Location, MouseOperates.LeftClick);
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        DoDragGraph = false;
        //GridDrawer.RedrawAsync(ClientWidth, ClientHeight, BackColor, e.Location);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (DoDragGraph)
        {
            var dX = e.X - DragStartPoint.X;
            var dY = e.Y - DragStartPoint.Y;
            GridDrawer.OffsetOrigin(new(dX, dY));
            DragStartPoint = e.Location;
            return;
        }
            // HACK: for test
            GridDrawer.PointOnCell(e.Location, MouseOperates.MoveOn);
        GridDrawer.DrawSelect(ClientSize, BackColor, e.Location);
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);
        var diffInWidth = e.Location.X - ClientWidth / 2;
        var diffInHeight = e.Location.Y - ClientHeight / 2;
        var dX = diffInWidth / GridDrawer.CellEdgeLength * ClientWidth / 200;
        var dY = diffInHeight / GridDrawer.CellEdgeLength * ClientHeight / 200;
        GridDrawer.CellEdgeLength += e.Delta / 100 * Math.Max(ClientWidth, ClientHeight) / 200;
        GridDrawer.OffsetOrigin(new(dX, dY));
    }

    protected override void OnMouseDoubleClick(MouseEventArgs e)
    {
        base.OnMouseDoubleClick(e);
        if (e.Button == MouseButtons.Left)
        {
            GridDrawer.PointOnCell(e.Location, MouseOperates.LeftDoubleClick);
            GridDrawer.DrawSelect(ClientSize, BackColor, e.Location);
            Invalidate();
        }
    }
}
