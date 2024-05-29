//#define MOUSE_DRAG_FREE

using LocalUtilities.TypeGeneral;
using WarringStates.Events;
using WarringStates.Graph;
using WarringStates.Map;

namespace WarringStates.UI.Component;

partial class GameDisplayer
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
        LocalEvents.Hub.AddListener<PointOnCellArgs>(LocalEvents.Graph.PointOnCell, PointOnCell);
    }

    private void PointOnCell(PointOnCellArgs args)
    {
        var land = args.TerrainPoint.GetLand();
        LocalEvents.Hub.Broadcast(LocalEvents.Test.AddSingleInfo, new TestForm.TestInfo("point", args.TerrainPoint.ToString()));
        LocalEvents.Hub.Broadcast(LocalEvents.Test.AddSingleInfo, new TestForm.TestInfo("terrain", land.Type.ToString()));
        LocalEvents.Hub.Broadcast(LocalEvents.Test.AddSingleInfo, new TestForm.TestInfo("cell part", args.PointOnCellPart.ToString()));
        if (land is SourceLand sourceLand)
            LocalEvents.Hub.Broadcast(LocalEvents.Test.AddSingleInfo, new TestForm.TestInfo("land part", sourceLand[args.TerrainPoint].ToString()));
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
        LocalEvents.Hub.TryBroadcast(LocalEvents.Graph.PointOnGameImage, args.Location);
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
            Relocate(dX, dY);
            DragStartPoint = args.Location;
        }
    }

    private void OnMouseWheel(object? sender, MouseEventArgs args)
    {
        var diffInWidth = args.Location.X - Width / 2;
        var diffInHeight = args.Location.Y - Height / 2;
        var dX = diffInWidth / LatticeGrid.CellEdgeLength * Width / 200;
        var dY = diffInHeight / LatticeGrid.CellEdgeLength * Height / 200;
        LatticeGrid.CellEdgeLength += args.Delta / 100 * Math.Max(Width, Height) / 200;
        Relocate(dX, dY);
    }
}
