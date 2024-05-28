using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Graph;
using LocalUtilities.TypeToolKit.Mathematic;
using WarringStates.Events;
using WarringStates.Graph;
using WarringStates.Map;

namespace WarringStates.UI.Component;

public class OverviewDisplayer : Displayer
{
    bool FullScreen { get; set; } = true;

    Bitmap? OverViewCache { get; set; }

    List<Rectangle> LastFocusOnRects { get; } = [];

    public void EnableListener()
    {
        LocalEvents.Hub.AddListener<GameDisplayerUpdateArgs>(LocalEvents.UserInterface.GameDisplayerUpdate, SetBounds);
        LocalEvents.Hub.AddListener<GridUpdatedArgs>(LocalEvents.Graph.GridUpdate, Relocate);
    }

    private void SetBounds(GameDisplayerUpdateArgs args)
    {
        if (Atlas.Overview is null)
            Size = new();
        else
        {
            var size = FullScreen ? args.DisplayRect.Size : new((int)(args.DisplayRect.Width * 0.25), (int)(args.DisplayRect.Height * 0.25));
            Size = Atlas.Overview.Size.ScaleSizeOnRatio(size);
            Location = FullScreen ? new((args.DisplayRect.Size - Size) / 2) : new(args.DisplayRect.Right - Width, args.DisplayRect.Top);
        }
    }

    private void Relocate(GridUpdatedArgs args)
    {
        if (Width is 0 || Height is 0 || Atlas.Overview is null)
            return;
        Relocate();
        if (OverViewCache is null || Size != OverViewCache.Size)
        {
            OverViewCache = Atlas.Overview.CopyToNewSize(Size);
            Image = Atlas.Overview.CopyToNewSize(Size);
        }
        OverViewCache.TemplateDrawPartsOn((Bitmap)Image, LastFocusOnRects, true);
        var width = args.DrawRect.Width / LatticeGrid.CellEdgeLength;
        var height = args.DrawRect.Height / LatticeGrid.CellEdgeLength;
        var x = Atlas.Width - args.Origin.X / LatticeGrid.CellEdgeLength;
        var y = Atlas.Height - args.Origin.Y / LatticeGrid.CellEdgeLength;
        var widthRatio = Atlas.Width / (double)Width;
        var heightRatio = Atlas.Height / (double)Height;
        var rect = new Rectangle((x / widthRatio).ToInt(), (y / heightRatio).ToInt(), (width / widthRatio).ToInt(), (height / heightRatio).ToInt());
        using var g = Graphics.FromImage(Image);
        var edges = rect.CutRectLoopEdgesInRange(new(new(0, 0), Size));
        LastFocusOnRects.Clear();
        foreach (var edge in edges)
        {
            g.DrawLine(Pens.Red, edge.Starter, edge.Ender);
            if (GetCrossEdgeRect(edge).CutRectInRange(new(new(0, 0), Size), out var r))
                LastFocusOnRects.Add(r.Value);
        }
        Invalidate();
    }

    private Rectangle GetCrossEdgeRect(Edge edge)
    {
        if (edge.Starter.Y == edge.Ender.Y)
        {
            if (edge.Starter.X == edge.Ender.X)
                return new();
            var y = edge.Starter.Y;
            var xMin = Math.Min(edge.Starter.X, edge.Ender.X);
            var width = Math.Abs(edge.Starter.X - edge.Ender.X);
            return new(xMin, y, width, 1);
        }
        var x = edge.Starter.X;
        var yMin = Math.Min(edge.Starter.Y, edge.Ender.Y);
        var height = Math.Abs(edge.Starter.Y - edge.Ender.Y);
        //
        // don't know why should +1 to height，
        // otherwise it will leave a "tail" on bottom-right when clearing last draw
        //
        return new(x, yMin, 1, height + 1);
    }
}
