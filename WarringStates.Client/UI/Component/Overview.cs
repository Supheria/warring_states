using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Graph;
using LocalUtilities.TypeToolKit.Mathematic;
using System.Drawing.Drawing2D;
using WarringStates.Client.Events;
using WarringStates.Client.Graph;
using WarringStates.Client.Map;

namespace WarringStates.Client.UI.Component;

public partial class Overview : Displayer
{
    bool FullScreen { get; set; } = false;

    Bitmap? OverviewCache { get; set; }

    Rectangle Range { get; set; }

    (double Width, double Height) FocusScaleRatio { get; set; }

    Rectangle FocusRect { get; set; }

    List<Rectangle> FocusRects { get; } = [];

    Color FocusColor { get; set; } = Color.Red;

    Rectangle GridDrawRect { get; set; } = new();

    Coordinate GridOrigin { get; set; } = new();

    public new Rectangle Bounds
    {
        get => base.Bounds;
        set
        {
            Visible = false;
            Range = value;
            if (FullScreen)
            {
                var size = GeometryTool.ScaleSizeWithinRatio(Atlas.Size, Range.Size);
                var location = new Point((int)(Range.Left + (Range.Width - size.Width) * 0.5f), (int)(Range.Top + (Range.Height - size.Height) * 0.5f));
                base.Bounds = new(location, size);
            }
            else
            {
                var size = new Size((int)(Range.Width * 0.25f), (int)(Range.Height * 0.25f));
                size = Atlas.Size.ScaleSizeWithinRatio(size);
                var location = new Point(Range.Right - size.Width, Range.Top);
                base.Bounds = new(location, size);
            }
            Visible = true;
        }
    }

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

    public override void Redraw()
    {
        Image?.Dispose();
        if (OverviewCache is not null && Size == OverviewCache.Size)
            Image = (Bitmap)OverviewCache.Clone();
        else
        {
            OverviewCache?.Dispose();
            OverviewCache = Atlas.GetOverview(ClientSize);
            var temp = OverviewCache.CopyToNewSize(ClientSize, InterpolationMode.Low);
            OverviewCache.Dispose();
            OverviewCache = temp;
            Image = (Bitmap)OverviewCache.Clone();
        }
        var edgeLength = (double)GridDrawer.CellEdgeLength;
        var width = GridDrawRect.Width / edgeLength;
        var height = GridDrawRect.Height / edgeLength;
        var x = Atlas.Width - GridOrigin.X / edgeLength;
        var y = Atlas.Height - GridOrigin.Y / edgeLength;
        var widthRatio = Atlas.Width / (double)ClientWidth;
        var heightRatio = Atlas.Height / (double)ClientHeight;
        FocusScaleRatio = (widthRatio * edgeLength, heightRatio * edgeLength);
        var focusRect = new Rectangle((x / widthRatio).ToRoundInt(), (y / heightRatio).ToRoundInt(), (width / widthRatio).ToRoundInt(), (height / heightRatio).ToRoundInt());
        using var g = Graphics.FromImage(Image);
        using var pen = new Pen(FocusColor, Math.Min(ClientWidth, ClientHeight) * 0.01f);
        foreach (var rect in GeometryTool.CutRectLoopRectsInRange(focusRect, ClientRect))
        {
            g.DrawRectangle(pen, rect);
        }
    }
}
