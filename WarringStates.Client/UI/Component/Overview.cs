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

    //GridRelocatedArgs? GridUpdatedArgs { get; set; }

    (double Width, double Height) FocusScaleRatio { get; set; }

    Rectangle FocusRect { get; set; }

    List<Rectangle> LastFocusOnRects { get; } = [];

    List<Rectangle> FocusRects { get; } = [];

    Color FocusColor { get; set; } = Color.Red;

    Rectangle GridDrawRect { get; set; } = new();

    Coordinate GridOrigin { get; set; } = new();

    public Overview()
    {
        AddOperations();
    }

    public void EnableListener()
    {
        //LocalEvents.TryAddListener<Rectangle>(LocalEvents.UserInterface.ToolBarOnSetBounds, SetBounds);
        LocalEvents.TryAddListener<GridRelocatedArgs>(LocalEvents.Graph.GridRedraw, Relocate);
    }

    public void DisableListener()
    {
        //LocalEvents.TryRemoveListener<Rectangle>(LocalEvents.UserInterface.ToolBarOnSetBounds, SetBounds);
        LocalEvents.TryRemoveListener<GridRelocatedArgs>(LocalEvents.Graph.GridRedraw, Relocate);
    }

    public void SetBounds(Rectangle bounds)
    {
        Range = bounds;
        if (FullScreen)
        {
            var size = GeometryTool.ScaleSizeWithinRatio(Atlas.Size, Range.Size);
            var location = new Point((int)(Range.Left + (Range.Width - size.Width) * 0.5f), (int)(Range.Top + (Range.Height - size.Height) * 0.5f));
            Bounds = new(location, size);
        }
        else
        {
            var size = new Size((int)(Range.Width * 0.25f), (int)(Range.Height * 0.25f));
            size = Atlas.Size.ScaleSizeWithinRatio(size);
            var location = new Point(Range.Right - size.Width, Range.Top);
            Bounds = new(location, size);
        }
        Redraw();
        Invalidate();
    }

    private void Relocate(GridRelocatedArgs args)
    {
        if (Width is 0 || Height is 0)
            return;
        GridDrawRect = args.DrawRect;
        GridOrigin = args.Origin;
        Redraw();
        Invalidate();
    }

    public override void Redraw()
    {
        RelocateOverview();
        RelocateFocus();
    }

    private void RelocateOverview()
    {
        if (OverviewCache is not null && Size == OverviewCache.Size)
        {
            BitmapTool.DrawTemplateSamePartsOnto(OverviewCache, (Bitmap)Image, LastFocusOnRects, true);
            return;
        }
        OverviewCache?.Dispose();
        Image?.Dispose();
        var widthUnit = (Width / (double)Atlas.Width).ToRoundInt();
        if (widthUnit is 0)
            widthUnit = 1;
        var heightUnit = (Height / (double)Atlas.Height).ToRoundInt();
        if (heightUnit is 0)
            heightUnit = 1;
        OverviewCache = new(Atlas.Width * widthUnit, Atlas.Height * heightUnit);
        var pOverview = new PointBitmap(OverviewCache);
        pOverview.LockBits();
        for (int i = 0; i < Atlas.Width; i++)
        {
            for (int j = 0; j < Atlas.Height; j++)
            {
                var color = Atlas.GetLand(new(i, j)).Color;
                drawUnit(i, j, color);
            }
        }
        pOverview.UnlockBits();
        var temp = OverviewCache.CopyToNewSize(Size, InterpolationMode.Low);
        OverviewCache.Dispose();
        OverviewCache = temp;
        Image = OverviewCache.Clone() as Bitmap;
        void drawUnit(int col, int row, Color color)
        {
            var dx = widthUnit * col;
            var dy = heightUnit * row;
            for (var x = 0; x < widthUnit; x++)
            {
                for (var y = 0; y < heightUnit; y++)
                {
                    pOverview.SetPixel(x + dx, y + dy, color);
                }
            }
        }
    }

    private void RelocateFocus()
    {
        var edgeLength = (double)GridDrawer.CellEdgeLength;
        var width = GridDrawRect.Width / edgeLength;
        var height = GridDrawRect.Height / edgeLength;
        var x = Atlas.Width - GridOrigin.X / edgeLength;
        var y = Atlas.Height - GridOrigin.Y / edgeLength;
        var widthRatio = Atlas.Width / (double)Width;
        var heightRatio = Atlas.Height / (double)Height;
        FocusScaleRatio = (widthRatio * edgeLength, heightRatio * edgeLength);
        FocusRect = new Rectangle((x / widthRatio).ToRoundInt(), (y / heightRatio).ToRoundInt(), (width / widthRatio).ToRoundInt(), (height / heightRatio).ToRoundInt());
        using var g = Graphics.FromImage(Image);
        FocusRects.Clear();
        FocusRects.AddRange(FocusRect.CutRectLoopRectsInRange(new(new(0, 0), Size)));
        using var pen = new Pen(FocusColor, Math.Min(Width, Height) * 0.01f);
        LastFocusOnRects.Clear();
        foreach (var rect in FocusRects)
        {
            g.DrawRectangle(pen, rect);
            foreach (var edge in EdgeTool.GetRectEdges(rect))
            {
                var lineRect = EdgeTool.GetCrossLineRect(edge, pen.Width);
                if (GeometryTool.CutRectInRange(lineRect, new(new(0, 0), Size), out var r))
                    LastFocusOnRects.Add(r.Value);
            }
        }
    }
}
