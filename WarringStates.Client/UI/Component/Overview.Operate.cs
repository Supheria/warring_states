using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using WarringStates.Client.Events;
using WarringStates.Client.Graph;
using WarringStates.Client.Map;

namespace WarringStates.Client.UI.Component;

partial class Overview
{
    public delegate void DrawImageHandler();

    bool DoDragFocus { get; set; } = false;

    Point DragStartPoint { get; set; } = new();

    public DrawImageHandler? OnDragImage { get; set; }

    static int DragMoveSensibility { get; set; } = GridDrawer.CellEdgeLength * 5;

    private void AddOperations()
    {
        MouseDoubleClick += OnMouseDoubleClick;
        MouseDown += OnMouseDown;
        MouseMove += OnMouseMove;
        MouseUp += OnMouseUp;
    }

    private void OnMouseDoubleClick(object? sender, MouseEventArgs args)
    {
        if (args.Button is MouseButtons.Left)
        {
            var dX = (args.X - FocusRect.Left - FocusRect.Width * 0.5) * FocusScaleRatio.Width;
            var dY = (args.Y - FocusRect.Top - FocusRect.Height * 0.5) * FocusScaleRatio.Height;
            GridDrawer.OffsetOrigin(new(-dX.ToRoundInt(), -dY.ToRoundInt()));
        }
        else if (args.Button is MouseButtons.Right)
        {
            //if (GridUpdatedArgs is null)
            //    return;
            FullScreen = !FullScreen;
            //Size = Atlas.Size.ScaleSizeWithinRatio(size);
            //Relocate(GridUpdatedArgs);
            //Location = FullScreen ? new(Range.Left + (Range.Width - Width) / 2, Range.Top + (Range.Height - Height) / 2) : new(Range.Right - Width, Range.Top);
            Bounds = Range;
        }
    }

    private void OnMouseDown(object? sender, MouseEventArgs args)
    {
        if (args.Button is MouseButtons.Left)
        {
            DoDragFocus = true;
            DragStartPoint = args.Location;
        }
    }

    private void OnMouseUp(object? sender, MouseEventArgs args)
    {
        if (DoDragFocus)
            DoDragFocus = false;
    }

    private void OnMouseMove(object? sender, MouseEventArgs args)
    {
        //LocalEvents.TryBroadcast(LocalEvents.Graph.GridCellPointedOn, args.Location);
        if (DoDragFocus)
        {
            var dX = (args.X - DragStartPoint.X) * FocusScaleRatio.Width;
            var dY = (args.Y - DragStartPoint.Y) * FocusScaleRatio.Height;
            //if (Math.Abs(dX) > DragMoveSensibility || Math.Abs(dY) > DragMoveSensibility)
            {
                //dX = dX / LatticeGrid.CellEdgeLength == 0 ? 0 : dX < 0 ? -1 : 1;
                //dX *= LatticeGrid.CellEdgeLength;
                //dY = dY / LatticeGrid.CellEdgeLength == 0 ? 0 : dY < 0 ? -1 : 1;
                //dY *= LatticeGrid.CellEdgeLength;
                GridDrawer.OffsetOrigin(new(-dX.ToRoundInt(), -dY.ToRoundInt()));
                DragStartPoint = args.Location;
                //var sendArgs = new GridOriginOperateArgs(GridOriginOperateArgs.OperateTypes.Offset, new(-dX.ToRoundInt(), -dY.ToRoundInt()));
                //LocalEvents.TryBroadcast(LocalEvents.Graph.OperateGridOrigin, sendArgs);
            }
            //var sendArgs = new GridOriginOperateArgs(GridOriginOperateArgs.OperateTypes.Offset, new(-dX.ToRoundInt(), -dY.ToRoundInt()));
            //LocalEvents.TryBroadcast(LocalEvents.Graph.OperateGridOrigin, sendArgs);
            //DragStartPoint = args.Location;
        }
    }
}
