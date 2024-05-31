using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using WarringStates.Events;
using WarringStates.Map;

namespace WarringStates.UI.Component;

partial class OverviewDisplayer
{
    protected override void AddOperations()
    {
        MouseDoubleClick += OnMouseDoubleClick;
        MouseMove += OnMouseMove;
    }

    private void OnMouseDoubleClick(object? sender, MouseEventArgs e)
    {
        if (e.Button is MouseButtons.Left)
        {
            var dX = (e.X - FocusRect.Left - FocusRect.Width * 0.5) * FocusScaleRatio.Width;
            var dY = (e.Y - FocusRect.Top - FocusRect.Height * 0.5) * FocusScaleRatio.Height;
            LocalEvents.Hub.Broadcast(LocalEvents.Graph.OffsetGridOrigin, new Coordinate(-dX.ToRoundInt(), -dY.ToRoundInt()));
        }
        else if (e.Button is MouseButtons.Right)
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

    private void OnMouseMove(object? sender, MouseEventArgs args)
    {
        LocalEvents.Hub.TryBroadcast(LocalEvents.Graph.PointOnGameImage, args.Location);
        switch (DragFlag)
        {
            case Directions.Left:
                var dX = (args.X - DragStartPoint.X) * FocusScaleRatio.Width;
                var dY = (args.Y - DragStartPoint.Y) * FocusScaleRatio.Height;
                LocalEvents.Hub.Broadcast(LocalEvents.Graph.OffsetGridOrigin, new Coordinate(-dX.ToRoundInt(), -dY.ToRoundInt()));
                break;
        }
    }
}
