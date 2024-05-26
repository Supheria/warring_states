//#define MOUSE_DRAG_FREE

using LocalUtilities.TypeGeneral;
using WarringStates.Graph;

namespace WarringStates.UI.Component;

partial class GameDisplayer
{
    bool DoDragGraph { get; set; } = false;

    Point DragStartPoint { get; set; } = new();

#if MOUSE_DRAG_FREE
    static int DragMoveSensibility = 1;
#else
    static int DragMoveSensibility => LatticeCell.CellData.EdgeLength;
#endif

    public OnComponentRunning? OnDragImage { get; set; }

    private void OnMouseDown(object? sender, MouseEventArgs args)
    {
        if (args.Button is MouseButtons.Left)
        {
            DoDragGraph = true;
            DragStartPoint = args.Location;
        }
    }

    private void OnMouseMove(object? sender, MouseEventArgs args)
    {
        if (DoDragGraph)
        {
            var dX = args.Location.X - DragStartPoint.X;
            var dY = args.Location.Y - DragStartPoint.Y;
            if (Math.Abs(dX) > DragMoveSensibility || Math.Abs(dY) > DragMoveSensibility)
            {
#if MOUSE_DRAG_FREE
                LatticeGrid.OriginX += (args.Location.X - DragStartPoint.X) * DragMoveSensibility;
                LatticeGrid.OriginY += (args.Location.Y - DragStartPoint.Y) * DragMoveSensibility;
#else
                dX = dX / DragMoveSensibility == 0 ? 0 : dX < 0 ? -1 : 1;
                dX *= DragMoveSensibility;
                dY = dY / DragMoveSensibility == 0 ? 0 : dY < 0 ? -1 : 1;
                dY *= DragMoveSensibility;
#endif
                Relocate(dX, dY);
                DragStartPoint = args.Location;
            }
        }
    }

    private void OnMouseUp(object? sender, MouseEventArgs args)
    {
        if (DoDragGraph)
            DoDragGraph = false;
    }
}
