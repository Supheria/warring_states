using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using WarringStates.Client.Events;
using WarringStates.Client.Map;

namespace WarringStates.Client.UI.Component;

partial class Overview
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
        if (args.Button is MouseButtons.Left)
        {
            var dX = (args.X - FocusRect.Left - FocusRect.Width * 0.5) * FocusScaleRatio.Width;
            var dY = (args.Y - FocusRect.Top - FocusRect.Height * 0.5) * FocusScaleRatio.Height;
            LocalEvents.TryBroadcast(LocalEvents.Graph.GridOriginToOffset, new Coordinate(-dX.ToRoundInt(), -dY.ToRoundInt()));
        }
        else if (args.Button is MouseButtons.Right)
        {
            if (GridUpdatedArgs is null)
                return;
            FullScreen = !FullScreen;
            var size = FullScreen ? Atlas.Size.ScaleSizeOnRatio(Range.Size) : new((Range.Width * 0.25).ToRoundInt(), (Range.Height * 0.25).ToRoundInt());
            Size = Atlas.Size.ScaleSizeOnRatio(size);
            Relocate(GridUpdatedArgs);
            Location = FullScreen ? new(Range.Left + (Range.Width - Width) / 2, Range.Top + (Range.Height - Height) / 2) : new(Range.Right - Width, Range.Top);
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
        LocalEvents.TryBroadcast(LocalEvents.Graph.GridCellPointedOn, args.Location);
        if (DoDragFocus)
        {
            var dX = (args.X - DragStartPoint.X) * FocusScaleRatio.Width;
            var dY = (args.Y - DragStartPoint.Y) * FocusScaleRatio.Height;
            LocalEvents.TryBroadcast(LocalEvents.Graph.GridOriginToOffset, new Coordinate(-dX.ToRoundInt(), -dY.ToRoundInt()));
            DragStartPoint = args.Location;
        }
    }
}
