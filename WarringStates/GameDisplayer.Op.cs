//#define MOUSE_DRAG_FREE

using LocalUtilities.TypeGeneral;

namespace WarringStates;

partial class GameDisplayer
{
    bool DoDragGraph { get; set; } = false;

    Point DragStartPoint { get; set; } = new();

#if MOUSE_DRAG_FREE
    static int DragMoveSensibility = 1;
#else
    static int DragMoveSensibility => LatticeCell.CellData.EdgeLength;
#endif

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
            var dWidth = args.Location.X - DragStartPoint.X;
            var dHeight = args.Location.Y - DragStartPoint.Y;
            if (Math.Abs(dWidth) > DragMoveSensibility || Math.Abs(dHeight) > DragMoveSensibility)
            {
#if MOUSE_DRAG_FREE
                LatticeGrid.OriginX += (args.Location.X - DragStartPoint.X) * DragMoveSensibility;
                LatticeGrid.OriginY += (args.Location.Y - DragStartPoint.Y) * DragMoveSensibility;
#else
                LatticeGrid.OriginX += (args.Location.X - DragStartPoint.X) / DragMoveSensibility * LatticeCell.CellData.EdgeLength;
                LatticeGrid.OriginY += (args.Location.Y - DragStartPoint.Y) / DragMoveSensibility * LatticeCell.CellData.EdgeLength;
#endif
                DragStartPoint = args.Location;
                Relocate();
            }
        }
    }

    private void OnMouseUp(object? sender, MouseEventArgs args)
    {
        if (DoDragGraph)
            DoDragGraph = false;
    }
}
