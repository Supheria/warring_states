using WarringStates.Server.Net;

namespace WarringStates.Server.UI.Component;

partial class Selector
{
    public event EventHandler? IndexChanged;

    enum DragPart
    {
        None,
        Item,
        Bar
    }

    DragPart Dragger { get; set; } = DragPart.None;

    Point DragStartPoint { get; set; } = new();

    protected override void OnMouseDoubleClick(MouseEventArgs e)
    {
        if (LocalNet.Server.IsStart)
            return;
        base.OnMouseDoubleClick(e);
        using var g = Graphics.FromImage(Image);
        var isContain = false;
        foreach (var rect in ItemRects)
        {
            if (rect.Contains(e.Location))
            {
                isContain = true;
                break;
            }
        }
        if (isContain)
            SelectedIndex = (e.Y - ItemColumnRect.Top - Padding.Height + Offset) / ItemHeight;
        else
            SelectedIndex = -1;
        Redraw();
        Invalidate();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (Dragger is DragPart.Bar)
        {
            ChangeOffset(e.Y - DragStartPoint.Y);
            Redraw();
            Invalidate();
        }
        DragStartPoint = e.Location;
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (ItemColumnRect.Contains(e.Location))
            Dragger = DragPart.Item;
        else if (BarRect.Contains(e.Location))
        {
            Dragger = DragPart.Bar;
            RollBarRedraw();
            Redraw();
            Invalidate();
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        Dragger = DragPart.None;
        RollBarRedraw();
        Redraw();
        Invalidate();
    }
}
