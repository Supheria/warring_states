using LocalUtilities.TypeToolKit.Mathematic;
using WarringStates.Client.Events;
using WarringStates.Client.Graph;

namespace WarringStates.Client.UI.Component;

partial class Overview
{
    public delegate void DrawImageHandler();

    bool DoDragFocus { get; set; } = false;

    Point DragStartPoint { get; set; } = new();

    public DrawImageHandler? OnDragImage { get; set; }

    public override void EnableListener()
    {
        base.EnableListener();
        LocalEvents.TryAddListener<GridRedrawArgs>(LocalEvents.Graph.GridRedraw, Relocate);
    }

    public override void DisableListener()
    {
        base.DisableListener();
        LocalEvents.TryRemoveListener<GridRedrawArgs>(LocalEvents.Graph.GridRedraw, Relocate);
    }

    private void Relocate(GridRedrawArgs args)
    {
        if (Width is 0 || Height is 0)
            return;
        GridDrawRect = args.DrawRect;
        GridOrigin = args.Origin;
        BeginInvoke(() =>
        {
            Redraw();
            Invalidate();
        });
    }

    protected override void OnMouseDoubleClick(MouseEventArgs e)
    {
        base.OnMouseDoubleClick(e);
        if (e.Button is MouseButtons.Left)
        {
            var dX = (e.X - FocusRect.Left - FocusRect.Width * 0.5) * FocusScaleRatio.Width;
            var dY = (e.Y - FocusRect.Top - FocusRect.Height * 0.5) * FocusScaleRatio.Height;
            GridDrawer.OffsetOrigin(new(-dX.ToRoundInt(), -dY.ToRoundInt()));
        }
        else if (e.Button is MouseButtons.Right)
        {
            FullScreen = !FullScreen;
            Bounds = Range;
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button is MouseButtons.Left)
        {
            DoDragFocus = true;
            DragStartPoint = e.Location;
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        if (DoDragFocus)
            DoDragFocus = false;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (DoDragFocus)
        {
            var dX = (e.X - DragStartPoint.X) * FocusScaleRatio.Width;
            var dY = (e.Y - DragStartPoint.Y) * FocusScaleRatio.Height;
            GridDrawer.OffsetOrigin(new(-dX.ToRoundInt(), -dY.ToRoundInt()));
            DragStartPoint = e.Location;
        }
    }
}
