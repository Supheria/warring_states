//#define MOUSE_DRAG_FREE

using LocalUtilities.TypeGeneral;
using WarringStates.Events;
using WarringStates.Graph;
using WarringStates.Map;
using WarringStates.Terrain;

namespace WarringStates.UI.Component;

partial class GamePlane
{
    bool DoDragGraph { get; set; } = false;

    Point DragStartPoint { get; set; } = new();

    static int DragMoveSensibility => LatticeGrid.CellEdgeLength;

    public OnComponentRunning? OnDragImage { get; set; }

    private void AddOperations()
    {
        MouseDown += OnMouseDown;
        MouseMove += OnMouseMove;
        MouseUp += OnMouseUp;
        MouseWheel += OnMouseWheel;
        LocalEvents.Hub.TryAddListener<GridCellPointedOnArgs>(LocalEvents.Graph.GridCellPointedOn, PointOnCell);
    }

    private void PointOnCell(GridCellPointedOnArgs args)
    {
        var land = args.TerrainPoint.GetLand();
        LocalEvents.Hub.TryBroadcast(LocalEvents.Test.AddSingleInfo, new TestForm.StringInfo("point", args.TerrainPoint.ToString()));
        LocalEvents.Hub.TryBroadcast(LocalEvents.Test.AddSingleInfo, new TestForm.StringInfo("terrain", land.Type.ToString()));
        LocalEvents.Hub.TryBroadcast(LocalEvents.Test.AddSingleInfo, new TestForm.StringInfo("cell part", args.PointOnCellPart.ToString()));
        if (land is SourceLand sourceLand)
            LocalEvents.Hub.TryBroadcast(LocalEvents.Test.AddSingleInfo, new TestForm.StringInfo("land part", sourceLand[args.TerrainPoint].ToString()));
    }

    private void OnMouseDown(object? sender, MouseEventArgs args)
    {
        if (args.Button is MouseButtons.Left)
        {
            DoDragGraph = true;
            DragStartPoint = args.Location;
        }
    }

    private void OnMouseUp(object? sender, MouseEventArgs args)
    {
        if (DoDragGraph)
            DoDragGraph = false;
    }

    private void OnMouseMove(object? sender, MouseEventArgs args)
    {
        LocalEvents.Hub.TryBroadcast(LocalEvents.Graph.GridCellToPointOn, args.Location);
        if (!DoDragGraph)
            return;
        var dX = args.X - DragStartPoint.X;
        var dY = args.Y - DragStartPoint.Y;
        if (Math.Abs(dX) > DragMoveSensibility || Math.Abs(dY) > DragMoveSensibility)
        {
            dX = dX / DragMoveSensibility == 0 ? 0 : dX < 0 ? -1 : 1;
            dX *= DragMoveSensibility;
            dY = dY / DragMoveSensibility == 0 ? 0 : dY < 0 ? -1 : 1;
            dY *= DragMoveSensibility;
            DragStartPoint = args.Location;
            LocalEvents.Hub.TryBroadcast(LocalEvents.Graph.GridOriginToOffset, new Coordinate(dX, dY));
        }
    }

    private void OnMouseWheel(object? sender, MouseEventArgs args)
    {
        var diffInWidth = args.Location.X - Width / 2;
        var diffInHeight = args.Location.Y - Height / 2;
        var dX = diffInWidth / LatticeGrid.CellEdgeLength * Width / 200;
        var dY = diffInHeight / LatticeGrid.CellEdgeLength * Height / 200;
        LatticeGrid.CellEdgeLength += args.Delta / 100 * Math.Max(Width, Height) / 200;
        LocalEvents.Hub.TryBroadcast(LocalEvents.Graph.GridOriginToOffset, new Coordinate(dX, dY));
    }
}
