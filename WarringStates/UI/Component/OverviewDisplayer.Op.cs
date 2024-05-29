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
    }

    private void OnMouseDoubleClick(object? sender, MouseEventArgs args)
    {
        if(args.Button is MouseButtons.Left)
        {
            var dX = (args.X - FocusRect.Left - FocusRect.Width * 0.5) * FocusScaleRatio.Width;
            var dY = (args.Y - FocusRect.Top - FocusRect.Height * 0.5) * FocusScaleRatio.Height;
            LocalEvents.Hub.Broadcast(LocalEvents.Graph.OffsetGridOrigin, new Coordinate(-dX.ToRoundInt(), -dY.ToRoundInt()));
        }
        else if (args.Button is MouseButtons.Right)
        {
            if (GridUpdatedArgs is null)
                return;
            FullScreen = !FullScreen;
            var size = FullScreen ? Atlas.Size.ScaleSizeOnRatio(DisplayRect.Size) : new((DisplayRect.Width * 0.25).ToRoundInt(), (DisplayRect.Height * 0.25).ToRoundInt());
            Size = Atlas.Size.ScaleSizeOnRatio(size);
            Relocate(GridUpdatedArgs);
            Location = FullScreen ? new(DisplayRect.Left + (DisplayRect.Width - Width) / 2, DisplayRect.Top + (DisplayRect.Height - Height) / 2) : new(DisplayRect.Right - Width, DisplayRect.Top);
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
        LocalEvents.Hub.TryBroadcast(LocalEvents.Graph.PointOnGameImage, args.Location);
        if (DoDragFocus)
        {
            var dX = (args.X - DragStartPoint.X) * FocusScaleRatio.Width;
            var dY = (args.Y - DragStartPoint.Y) * FocusScaleRatio.Height;
            LocalEvents.Hub.Broadcast(LocalEvents.Graph.OffsetGridOrigin, new Coordinate(-dX.ToRoundInt(), -dY.ToRoundInt()));
            DragStartPoint = args.Location;
        }
    }
}
