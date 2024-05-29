using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Events;
using WarringStates.Graph;
using WarringStates.Map;

namespace WarringStates.UI.Component;

partial class OverviewDisplayer
{
    bool DoDragFocus { get; set; } = false;

    Point DragStartPoint { get; set; } = new();

    public OnComponentRunning? OnDragImage { get; set; }

    private void AddOperations()
    {
        MouseDoubleClick += OnMouseDoubleClick;
        MouseDown += OnMouseDown;
        MouseMove += OnMouseMove;
        MouseUp += OnMouseUp;
        MouseWheel += OnMouseWheel;
    }

    private void OnMouseDoubleClick(object? sender, MouseEventArgs args)
    {
        var dX = (args.X - FocusRect.Left - FocusRect.Width * 0.5) * FocusScaleRatio.Width * LatticeGrid.CellEdgeLength;
        var dY = (args.Y - FocusRect.Top - FocusRect.Height * 0.5) * FocusScaleRatio.Height * LatticeGrid.CellEdgeLength;
        LocalEvents.Hub.Broadcast(LocalEvents.Graph.OffsetGridOrigin, new Coordinate(-dX.ToRoundInt(), -dY.ToRoundInt()));
    }

    private void OnMouseDown(object? sender, MouseEventArgs args)
    {
        if (args.Button is MouseButtons.Left)
        {
            if (FocusRects.Any(r=>r.Contains(args.X, args.Y)))
            {
                DoDragFocus = true;
                DragStartPoint = args.Location;
            }
            else
            {
            }
        }
    }

    private void OnMouseUp(object? sender, MouseEventArgs args)
    {
        if (DoDragFocus)
            DoDragFocus = false;
    }

    private void OnMouseMove(object? sender, MouseEventArgs args)
    {
        LocalEvents.Hub.TryBroadcast(LocalEvents.Graph.PointOnGameImage, args.Location);
        if (!DoDragFocus)
        {
        }
        else
        {

            //var dX = args.Location.X - DragStartPoint.X;
            //var dY = args.Location.Y - DragStartPoint.Y;
            //if (Math.Abs(dX) > DragMoveSensibility || Math.Abs(dY) > DragMoveSensibility)
            //{
            //    dX = dX / DragMoveSensibility == 0 ? 0 : dX < 0 ? -1 : 1;
            //    dX *= DragMoveSensibility;
            //    dY = dY / DragMoveSensibility == 0 ? 0 : dY < 0 ? -1 : 1;
            //    dY *= DragMoveSensibility;
            //    Relocate(dX, dY);
            //    DragStartPoint = args.Location;
            //}
        }
    }

    private void OnMouseWheel(object? sender, MouseEventArgs args)
    {
        var diffInWidth = args.Location.X - Width / 2;
        var diffInHeight = args.Location.Y - Height / 2;
        var dX = diffInWidth / LatticeGrid.CellEdgeLength * Width / 200;
        var dY = diffInHeight / LatticeGrid.CellEdgeLength * Height / 200;
        LatticeGrid.CellEdgeLength += args.Delta / 100 * Math.Max(Width, Height) / 200;
        //Relocate(dX, dY);
    }
}
