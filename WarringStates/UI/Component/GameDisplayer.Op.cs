//#define MOUSE_DRAG_FREE

using LocalUtilities.TypeGeneral;
using WarringStates.Events;
using WarringStates.Graph;
using WarringStates.Map;
using WarringStates.Terrain;

namespace WarringStates.UI.Component;

partial class GameDisplayer
{
    static int DragMoveSensibility => LatticeGrid.CellEdgeLength;

    public OnComponentRunning? OnDragImage { get; set; }

    protected override void AddOperations()
    {
        MouseMove += OnMouseMove;
        MouseWheel += OnMouseWheel;
        LocalEvents.Hub.AddListener<PointOnCellArgs>(LocalEvents.Graph.PointOnCell, PointOnCell);
    }

    private void PointOnCell(PointOnCellArgs args)
    {
        var land = args.TerrainPoint.GetLand();
        LocalEvents.Hub.Broadcast(LocalEvents.Test.AddSingleInfo, new TestForm.StringInfo("point", args.TerrainPoint.ToString()));
        LocalEvents.Hub.Broadcast(LocalEvents.Test.AddSingleInfo, new TestForm.StringInfo("terrain", land.Type.ToString()));
        LocalEvents.Hub.Broadcast(LocalEvents.Test.AddSingleInfo, new TestForm.StringInfo("cell part", args.PointOnCellPart.ToString()));
        if (land is SourceLand sourceLand)
            LocalEvents.Hub.Broadcast(LocalEvents.Test.AddSingleInfo, new TestForm.StringInfo("land part", sourceLand[args.TerrainPoint].ToString()));
    }

    private void OnMouseMove(object? sender, MouseEventArgs e)
    {
        LocalEvents.Hub.TryBroadcast(LocalEvents.Graph.PointOnGameImage, e.Location);
        switch (DragFlag)
        {
            case Directions.Left:
                var dX = e.X - DragStartPoint.X;
                var dY = e.Y - DragStartPoint.Y;
                if (Math.Abs(dX) > DragMoveSensibility || Math.Abs(dY) > DragMoveSensibility)
                {
                    dX = dX / DragMoveSensibility == 0 ? 0 : dX < 0 ? -1 : 1;
                    dX *= DragMoveSensibility;
                    dY = dY / DragMoveSensibility == 0 ? 0 : dY < 0 ? -1 : 1;
                    dY *= DragMoveSensibility;
                    Relocate(dX, dY);
                }
                break;
        }
    }

    private void OnMouseWheel(object? sender, MouseEventArgs e)
    {
        var diffInWidth = e.Location.X - Width / 2;
        var diffInHeight = e.Location.Y - Height / 2;
        var dX = diffInWidth / LatticeGrid.CellEdgeLength * Width / 200;
        var dY = diffInHeight / LatticeGrid.CellEdgeLength * Height / 200;
        LatticeGrid.CellEdgeLength += e.Delta / 100 * Math.Max(Width, Height) / 200;
        Relocate(dX, dY);
    }
}
