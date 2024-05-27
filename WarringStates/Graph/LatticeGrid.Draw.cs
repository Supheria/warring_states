using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using System.Diagnostics;
using System.Drawing;
using WarringStates.Events;
using WarringStates.Map;
using WarringStates.UI;

namespace WarringStates.Graph;

partial class LatticeGrid
{
    Rectangle LastDrawRect { get; set; } = new();

    Coordinate LastOrigin { get; set; } = new();

    Dictionary<Coordinate, Color> LastCellColor { get; set; } = [];

    Rectangle[] LastGuideLineRects { get; set; } = new Rectangle[2];

    int LastCellEdgeLength { get; set; } = LatticeCell.CellData.EdgeLength;

    Color BackColor { get; set; }

    private void DrawGrid(GameImageUpdateArgs args)
    {
        ResetOrigin(args.OriginOffset);
        DrawRect = new(new(0, 0), args.Source.Size);
        Graphics = Graphics.FromImage(args.Source);
        BackColor = args.BackColor;
        var stop = new Stopwatch();
        stop.Start();
        DrawLatticeCells();
        DrawGuideLine();
        Graphics.Flush();
        Graphics.Dispose();
        stop.Stop();
        LastOrigin = Origin;
        LastDrawRect = DrawRect;
        LastCellEdgeLength = LatticeCell.CellData.EdgeLength;
        LocalEvents.Hub.Broadcast(LocalEvents.Test.AddInfoForMax, new TestForm.TestInfo("use time", stop.ElapsedMilliseconds.ToString()));
        LocalEvents.Hub.Broadcast(LocalEvents.Graph.GridUpdate, new GridUpdatedArgs(DrawRect, Origin));
    }

    private void ResetOrigin(Coordinate offset)
    {
        var width = Terrain.Width * LatticeCell.CellData.EdgeLength;
        var x = (Origin.X + offset.X) % width;
        if (x < 0)
            x += width;
        var height = Terrain.Height * LatticeCell.CellData.EdgeLength;
        var y = (Origin.Y + offset.Y) % height;
        if (y < 0)
            y += height;
        Origin = new(x, y);
    }

    private void DrawGuideLine()
    {
        var brush = new SolidBrush(BackColor);
        Graphics?.FillRectangle(brush, LastGuideLineRects[0]);
        Graphics?.FillRectangle(brush, LastGuideLineRects[1]);
        brush.Color = GridData.GuideLineColor;
        var lineRect = GetCrossLineRect(new(Origin.X, DrawRect.Top), new(Origin.X, DrawRect.Bottom), GridData.GuideLineWidth);
        Graphics?.FillRectangle(brush, lineRect);
        LastGuideLineRects[0] = lineRect;
        lineRect = GetCrossLineRect(new(DrawRect.Left, Origin.Y), new(DrawRect.Right, Origin.Y), GridData.GuideLineWidth);
        LastGuideLineRects[1] = lineRect;
        Graphics?.FillRectangle(brush, lineRect);
    }

    public Rectangle GetCrossLineRect(Coordinate p1, Coordinate p2, double lineWidth)
    {
        Rectangle lineRect;
        if (p1.Y == p2.Y)
        {
            var y = p1.Y - lineWidth / 2;
            var xMin = Math.Min(p1.X, p2.X);
            lineRect = new(xMin, (int)y, Math.Abs(p1.X - p2.X), (int)lineWidth);
        }
        else
        {
            var x = p1.X - lineWidth / 2;
            var yMin = Math.Min(p1.Y, p2.Y);
            lineRect = new((int)x, yMin, (int)lineWidth, Math.Abs(p1.Y - p2.Y));
        }
        if (lineRect.CutRectInRange(DrawRect, out var result))
            return result.Value;
        return new();
    }

    private void DrawLatticeCells()
    {
        var edgeLength = LatticeCell.CellData.EdgeLength;
        var colOffset = Origin.X / LatticeCell.CellData.EdgeLength + 1;
        var rowOffset = Origin.Y / LatticeCell.CellData.EdgeLength + 1;
        var colNumber = DrawRect.Width / LatticeCell.CellData.EdgeLength + 2;
        var rowNumber = DrawRect.Height / LatticeCell.CellData.EdgeLength + 2;
        var dX = Origin.X - LastOrigin.X;
        var dY = Origin.Y - LastOrigin.Y;
        var cellBrush = new Dictionary<Color, SolidBrush>();
        var count = 0;
        //
        // redraw all
        //
        if (DrawRect != LastDrawRect ||
            DrawRect.Height > LastDrawRect.Height ||
            dX % edgeLength != 0 || dY % edgeLength != 0 ||
            LastCellEdgeLength != LatticeCell.CellData.EdgeLength)
        {
            Graphics?.Clear(BackColor);
            for (var i = 0; i < colNumber; i++)
            {
                for (var j = 0; j < rowNumber; j++)
                {
                    var cell = new LatticeCell(Origin, new Coordinate(i - colOffset, j - rowOffset));
                    var color = cell.TerrainPoint.GetTerrain().GetColor();
                    if (!cellBrush.TryGetValue(color, out SolidBrush? brush))
                        brush = cellBrush[color] = new SolidBrush(color);
                    Graphics?.FillRectangle(brush, cell.CenterRealRect);
                    count++;
                }
            }
            return;
        }
        //
        // redraw changed
        //
        dX /= edgeLength;
        dY /= edgeLength;
        for (var i = 0; i < colNumber; i++)
        {
            for (var j = 0; j < rowNumber; j++)
            {
                var point = new Coordinate(i - colOffset, j - rowOffset);
                var cell = new LatticeCell(Origin, point);
                var color = cell.TerrainPoint.GetTerrain().GetColor();
                var lastPoint = new Coordinate(point.X + dX, point.Y + dY);
                if (!LastCellColor.TryGetValue(lastPoint, out var lastColor) || color != lastColor)
                {
                    if (cell.CenterRealRect.CutRectInRange(DrawRect, out var rect))
                    {
                        if (!cellBrush.TryGetValue(color, out SolidBrush? brush))
                            brush = cellBrush[color] = new SolidBrush(color);
                        Graphics?.FillRectangle(brush, rect.Value);
                        count++;
                    }
                }
                LastCellColor[point] = color;
            }
        }
        LocalEvents.Hub.Broadcast(LocalEvents.Test.AddSingleInfo, new TestForm.TestInfo("draw cell count", count.ToString()));
    }
}
