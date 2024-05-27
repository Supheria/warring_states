using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using WarringStates.Events;
using WarringStates.Map;
using WarringStates.UI;

namespace WarringStates.Graph;

partial class LatticeGrid
{
    Rectangle LastDrawRect { get; set; } = new();

    Coordinate OriginOffset { get; set; } = new();

    Rectangle[] LastGuideLineRects { get; } = new Rectangle[2];

    int LastCellEdgeLength { get; set; } = CellData.EdgeLength;

    Color BackColor { get; set; }

    Dictionary<Color, SolidBrush> CellBrush { get; } = [];

    public Size LatticeSize { get; set; } = new();

    public Size LatticeOffset { get; set; } = new();

    private void DrawGrid(GameImageUpdateArgs args)
    {
        var lastOrigin = Origin;
        ResetOrigin(args.OriginOffset);
        OriginOffset = Origin - lastOrigin;
        DrawRect = new(new(0, 0), args.Source.Size);
        Graphics = Graphics.FromImage(args.Source);
        BackColor = args.BackColor;
        DrawGrid();
        Graphics.Flush();
        Graphics.Dispose();
        LastDrawRect = DrawRect;
        LastCellEdgeLength = CellData.EdgeLength;
        LocalEvents.Hub.Broadcast(LocalEvents.Graph.GridUpdate, new GridUpdatedArgs(DrawRect, Origin));
    }

    private void ResetOrigin(Coordinate offset)
    {
        var width = Terrain.Width * CellData.EdgeLength;
        var x = (Origin.X + offset.X) % width;
        if (x < 0)
            x += width;
        var height = Terrain.Height * CellData.EdgeLength;
        var y = (Origin.Y + offset.Y) % height;
        if (y < 0)
            y += height;
        Origin = new(x, y);
    }

    private void DrawGrid()
    {
        Cell.GridOrigin = Origin;
        LatticeSize = new(DrawRect.Width / CellData.EdgeLength + 2, DrawRect.Height / CellData.EdgeLength + 2);
        LatticeOffset = new(Origin.X / CellData.EdgeLength + 1, Origin.Y / CellData.EdgeLength + 1);
        var count = 0;
        //
        // redraw all
        //
        if (DrawRect != LastDrawRect ||
            OriginOffset.X % CellData.EdgeLength != 0 || OriginOffset.Y % CellData.EdgeLength != 0 ||
            LastCellEdgeLength != CellData.EdgeLength)
        {
            Graphics?.Clear(BackColor);
            DrawGuideLine();
            for (var i = 0; i < LatticeSize.Width; i++)
            {
                for (var j = 0; j < LatticeSize.Height; j++)
                {
                    var cell = new Cell(new(i - LatticeOffset.Width, j - LatticeOffset.Height));
                    var color = cell.TerrainPoint.GetTerrain().GetColor();
                    if (!cell.CenterRealRect.CutRectInRange(DrawRect, out var rect))
                        continue;
                    if (!CellBrush.TryGetValue(color, out var brush))
                        brush = CellBrush[color] = new SolidBrush(color);
                    Graphics?.FillRectangle(brush, rect.Value);
                    count++;
                }
            }
            LocalEvents.Hub.Broadcast(LocalEvents.Test.AddSingleInfo, new TestForm.TestInfo("redraw cell count (all)", count.ToString()));
            return;
        }
        //
        // redraw changed only
        //
        GridData.GuideLineBrush.Color = BackColor;
        Graphics?.FillRectangle(GridData.GuideLineBrush, LastGuideLineRects[0]);
        Graphics?.FillRectangle(GridData.GuideLineBrush, LastGuideLineRects[1]);
        DrawGuideLine();
        DrawLatticeCells();
    }

    private void DrawGuideLine()
    {
        GridData.GuideLineBrush.Color = GridData.GuideLineColor;
        var lineRect = GetLineRect(new(Origin.X, DrawRect.Top), new(Origin.X, DrawRect.Bottom), GridData.GuideLineWidth);
        Graphics?.FillRectangle(GridData.GuideLineBrush, lineRect);
        LastGuideLineRects[0] = lineRect;
        lineRect = GetLineRect(new(DrawRect.Left, Origin.Y), new(DrawRect.Right, Origin.Y), GridData.GuideLineWidth);
        LastGuideLineRects[1] = lineRect;
        Graphics?.FillRectangle(GridData.GuideLineBrush, lineRect);
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

    private void DrawLatticeCells()
    {
        var count = 0;
        var dX = OriginOffset.X / CellData.EdgeLength;
        var dY = OriginOffset.Y / CellData.EdgeLength;
        for (var i = 0; i < LatticeSize.Width; i++)
        {
            for (var j = 0; j < LatticeSize.Height; j++)
            {
                var point = new Coordinate(i - LatticeOffset.Width, j - LatticeOffset.Height);
                var cell = new Cell(point);
                var color = cell.TerrainPoint.GetTerrain().GetColor();
                var lastCell = new Cell(new(point.X + dX, point.Y + dY));
                if (lastCell.TerrainPoint.GetTerrain().GetColor() == color)
                    continue;
                if (!cell.CenterRealRect.CutRectInRange(DrawRect, out var rect))
                    continue;
                if (!CellBrush.TryGetValue(color, out var brush))
                    brush = CellBrush[color] = new SolidBrush(color);
                Graphics?.FillRectangle(brush, rect.Value);
                count++;
            }
        }
        LocalEvents.Hub.Broadcast(LocalEvents.Test.AddSingleInfo, new TestForm.TestInfo("redraw cell count (part)", count.ToString()));
    }
}
