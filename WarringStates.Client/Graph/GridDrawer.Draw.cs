using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using System.Diagnostics;
using WarringStates.Client.Events;
using WarringStates.Client.Map;
using WarringStates.Client.UI;
using WarringStates.Map.Terrain;

namespace WarringStates.Client.Graph;

partial class GridDrawer
{
    static Rectangle DrawRect { get; set; }

    static Rectangle[] LastGuideLineRects { get; } = new Rectangle[2];

    static Color BackColor { get; set; }

    public static Size LatticeSize { get; set; } = new();

    public static Size LatticeOffset { get; set; } = new();

    static bool IsDrawing { get; set; } = false;

    static bool IsWaiting { get; set; } = false;

    public static void OffsetOrigin(Coordinate offset)
    {
        var width = Atlas.Width * CellEdgeLength;
        var x = (Origin.X + offset.X) % width;
        if (x < 0)
            x += width;
        var height = Atlas.Height * CellEdgeLength;
        var y = (Origin.Y + offset.Y) % height;
        if (y < 0)
            y += height;
        Origin = new(x, y);
        LocalEvents.TryBroadcast(LocalEvents.Graph.GridOriginReset);
    }

    public static async void RedrawAsync(int width, int height, Color backColor)
    {
        if (IsDrawing)
        {
            IsWaiting = true;
            return;
        }
        IsDrawing = true;
        var source = new Bitmap(width, height);
        var task = Task.Run(() => Redraw(source, backColor));
        await task;
        var sendArgs = new GridRedrawArgs(source, DrawRect, Origin);
        LocalEvents.TryBroadcast(LocalEvents.Graph.GridRedraw, sendArgs);
        IsDrawing = false;
        if (IsWaiting)
        {
            RedrawAsync(width, height, backColor);
            IsWaiting = false;
        }
    }

    private static void Redraw(Image source, Color backColor)
    {
        DrawRect = new(new(0, 0), source.Size);
        using var g = Graphics.FromImage(source);
        BackColor = backColor;
        DrawGrid(g);
    }

    private static void DrawGrid(Graphics g)
    {
        Cell.GridOrigin = Origin;
        LatticeSize = new(DrawRect.Width / CellEdgeLength + 2, DrawRect.Height / CellEdgeLength + 2);
        LatticeOffset = new(Origin.X / CellEdgeLength + 1, Origin.Y / CellEdgeLength + 1);
        g.Clear(BackColor);
        for (var i = 0; i < LatticeSize.Width; i++)
        {
            for (var j = 0; j < LatticeSize.Height; j++)
            {
                var cell = new Cell(new(i - LatticeOffset.Width, j - LatticeOffset.Height));
                var land = cell.TerrainPoint.GetLand();
                DrawLand(g, land, cell);
            }
        }
        DrawGuideLine(g);
    }

    private static void DrawLand(Graphics g, ILand land, Cell cell)
    {
        if (land is SingleLand)
        {
            if (cell.CenterRealRect.CutRectInRange(DrawRect, out var rect))
                g.FillRectangle(new SolidBrush(land.Color), rect.Value);
        }
        else if (land is SourceLand sourceLand)
        {
            var direction = sourceLand[cell.TerrainPoint];
            if (GetSourceLandCellRect(direction, cell).CutRectInRange(DrawRect, out var rect))
                g.FillRectangle(new SolidBrush(land.Color), rect.Value);
        }
    }

    private static Rectangle GetSourceLandCellRect(Directions direction, Cell cell)
    {
        return direction switch
        {
            Directions.LeftTop => new(new(cell.CenterRealRect.Left, cell.CenterRealRect.Top), CellCenterSizeAddOnePadding),
            Directions.Top => new(cell.RealRect.Left, cell.CenterRealRect.Top, CellEdgeLength, CellCenterSizeAddOnePadding.Height),
            Directions.TopRight => new(new(cell.RealRect.Left, cell.CenterRealRect.Top), CellCenterSizeAddOnePadding),
            Directions.Left => new(cell.CenterRealRect.Left, cell.RealRect.Top, CellCenterSizeAddOnePadding.Width, CellEdgeLength),
            Directions.Center => cell.RealRect,
            Directions.Right => new(cell.RealRect.Left, cell.RealRect.Top, CellCenterSizeAddOnePadding.Width, CellEdgeLength),
            Directions.LeftBottom => new(new(cell.CenterRealRect.Left, cell.RealRect.Top), CellCenterSizeAddOnePadding),
            Directions.Bottom => new(cell.RealRect.Left, cell.RealRect.Top, CellEdgeLength, CellCenterSizeAddOnePadding.Height),
            Directions.BottomRight => new(new(cell.RealRect.Left, cell.RealRect.Top), CellCenterSizeAddOnePadding),
            _ => new()
        };
    }

    private static void DrawGuideLine(Graphics g)
    {
        GridData.GuideLineBrush.Color = GridData.GuideLineColor;
        var lineRect = GetLineRect(new(Origin.X, DrawRect.Top), new(Origin.X, DrawRect.Bottom), GridData.GuideLineWidth);
        g.FillRectangle(GridData.GuideLineBrush, lineRect);
        LastGuideLineRects[0] = lineRect;
        lineRect = GetLineRect(new(DrawRect.Left, Origin.Y), new(DrawRect.Right, Origin.Y), GridData.GuideLineWidth);
        LastGuideLineRects[1] = lineRect;
        g.FillRectangle(GridData.GuideLineBrush, lineRect);
        static Rectangle GetLineRect(Coordinate p1, Coordinate p2, double lineWidth)
        {
            if (p1.Y == p2.Y)
            {
                var y = p1.Y - lineWidth / 2;
                var xMin = Math.Min(p1.X, p2.X);
                return new(xMin, (int)y, Math.Abs(p1.X - p2.X), (int)lineWidth);
            }
            else
            {
                var x = p1.X - lineWidth / 2;
                var yMin = Math.Min(p1.Y, p2.Y);
                return new((int)x, yMin, (int)lineWidth, Math.Abs(p1.Y - p2.Y));
            }
        }
    }
}
