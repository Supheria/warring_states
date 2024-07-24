//#define MOUSE_DRAG_FREE

using LocalUtilities.TypeGeneral;
using WarringStates.Client.Events;
using WarringStates.Client.Graph;
using WarringStates.Client.Map;
using WarringStates.Map.Terrain;

namespace WarringStates.Client.UI.Component;

partial class GamePlane
{
    public delegate void DrawImageHandler();

    bool DoDragGraph { get; set; } = false;

    Point DragStartPoint { get; set; } = new();

    static int DragMoveSensibility { get; set; } = GridDrawer.CellEdgeLength;

    public DrawImageHandler? OnDragImage { get; set; }

    private void AddOperations()
    {
        MouseDown += OnMouseDown;
        MouseMove += OnMouseMove;
        MouseUp += OnMouseUp;
        MouseWheel += OnMouseWheel;
        LocalEvents.TryAddListener<GridCellPointedOnArgs>(LocalEvents.Graph.PointOnCell, PointOnCell);
    }

    private void PointOnCell(GridCellPointedOnArgs args)
    {
        var land = args.TerrainPoint.GetLand();
        LocalEvents.TryBroadcast(LocalEvents.Test.AddSingleInfo, new TestForm.StringInfo("point", args.TerrainPoint.ToString()));
        LocalEvents.TryBroadcast(LocalEvents.Test.AddSingleInfo, new TestForm.StringInfo("terrain", land.Type.ToString()));
        LocalEvents.TryBroadcast(LocalEvents.Test.AddSingleInfo, new TestForm.StringInfo("cell part", args.PointOnCellPart.ToString()));
        if (land is SourceLand sourceLand)
            LocalEvents.TryBroadcast(LocalEvents.Test.AddSingleInfo, new TestForm.StringInfo("land part", sourceLand[args.TerrainPoint].ToString()));
    }

    private void OnMouseDown(object? sender, MouseEventArgs args)
    {
        if (args.Button is MouseButtons.Left)
        {
            DoDragGraph = true;
            DragStartPoint = args.Location;
            GridDrawer.PointOnCell(args.Location);
        }
    }

    private void OnMouseUp(object? sender, MouseEventArgs args)
    {
        if (DoDragGraph)
            DoDragGraph = false;
    }

    private void OnMouseMove(object? sender, MouseEventArgs args)
    {
        if (!DoDragGraph)
        {
            // HACK: for test
            GridDrawer.PointOnCell(args.Location);
            return;
        }
        var dX = args.X - DragStartPoint.X;
        var dY = args.Y - DragStartPoint.Y;
        //if (Math.Abs(dX) > DragMoveSensibility || Math.Abs(dY) > DragMoveSensibility)
        {
            //dX = dX / DragMoveSensibility == 0 ? 0 : dX < 0 ? -1 : 1;
            //dX *= DragMoveSensibility;
            //dY = dY / DragMoveSensibility == 0 ? 0 : dY < 0 ? -1 : 1;
            //dY *= DragMoveSensibility;
            DragStartPoint = args.Location;
            GridDrawer.OffsetOrigin(new(dX, dY));
        }
    }

    private void OnMouseWheel(object? sender, MouseEventArgs args)
    {
        var diffInWidth = args.Location.X - ClientWidth / 2;
        var diffInHeight = args.Location.Y - ClientHeight / 2;
        var dX = diffInWidth / GridDrawer.CellEdgeLength * ClientWidth / 200;
        var dY = diffInHeight / GridDrawer.CellEdgeLength * ClientHeight / 200;
        GridDrawer.CellEdgeLength += args.Delta / 100 * Math.Max(ClientWidth, ClientHeight) / 200;
        GridDrawer.OffsetOrigin(new(dX, dY));
    }
}
