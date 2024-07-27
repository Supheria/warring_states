//#define MOUSE_DRAG_FREE

using WarringStates.Client.Events;
using WarringStates.Client.Graph;
using WarringStates.Client.Map;

namespace WarringStates.Client.UI.Component;

partial class GamePlane
{
    public delegate void DrawImageHandler();

    bool DoDragGraph { get; set; } = false;

    Point DragStartPoint { get; set; } = new();

    public DrawImageHandler? OnDragImage { get; set; }

    private void PointOnCell(GridCellPointedOnArgs args)
    {
        var land = args.TerrainPoint.GetLand();
        LocalEvents.TryBroadcast(LocalEvents.Test.AddSingleInfo, new TestForm.StringInfo("point", args.TerrainPoint.ToString()));
        LocalEvents.TryBroadcast(LocalEvents.Test.AddSingleInfo, new TestForm.StringInfo("terrain", land.LandType.ToString()));
        LocalEvents.TryBroadcast(LocalEvents.Test.AddSingleInfo, new TestForm.StringInfo("cell part", args.PointOnCellPart.ToString()));
        //if (land is SourceLand sourceLand)
        //    LocalEvents.TryBroadcast(LocalEvents.Test.AddSingleInfo, new TestForm.StringInfo("land part", sourceLand[args.TerrainPoint].ToString()));
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button is MouseButtons.Left)
        {
            DoDragGraph = true;
            DragStartPoint = e.Location;
            GridDrawer.PointOnCell(e.Location);
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        DoDragGraph = false;
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
        }
        else
            // HACK: for test
            GridDrawer.PointOnCell(e.Location);
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
}
