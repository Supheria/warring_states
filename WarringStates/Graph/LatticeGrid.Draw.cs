using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using System.Diagnostics;
using WarringStates.Events;
using WarringStates.Map;
using WarringStates.UI;

namespace WarringStates.Graph;

partial class LatticeGrid
{
    private void DrawGrid(GameImageUpdateArgs args)
    {
        var width = Terrain.Width * LatticeCell.CellData.EdgeLength;
        var x = (Origin.X + args.OriginOffset.X) % width;
        if (x < 0)
            x += width;
        var height = Terrain.Height * LatticeCell.CellData.EdgeLength;
        var y = (Origin.Y + args.OriginOffset.Y) % height;
        if (y < 0)
            y += height;
        Origin = new(x, y);
        DrawRect = new(new(0, 0), args.Source.Size);
        Graphics = Graphics.FromImage(args.Source);
        var stop = new Stopwatch();
        Graphics.Clear(args.BackColor);
        stop.Start();
        DrawLatticeCells();
        DrawGuideLine();
        Graphics.Flush();
        Graphics.Dispose();
        stop.Stop();
        LocalEvents.Hub.Broadcast(LocalEvents.Test.AddSingleInfo, new TestForm.TestInfo("use time", stop.ElapsedMilliseconds.ToString()));
        LocalEvents.Hub.Broadcast(LocalEvents.Graph.GridUpdate, new GridUpdatedArgs(DrawRect, Origin));
    }

    private void DrawGuideLine()
    {
        var brush = new SolidBrush(GridData.GuideLineColor);
        var lineRect = GetCrossLineRect(new(Origin.X, DrawRect.Top), new(Origin.X, DrawRect.Bottom), GridData.GuideLineWidth);
        Graphics?.FillRectangle(brush, lineRect);
        lineRect = GetCrossLineRect(new(DrawRect.Left, Origin.Y), new(DrawRect.Right, Origin.Y), GridData.GuideLineWidth);
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
        var colOffset = Origin.X / LatticeCell.CellData.EdgeLength + 1;
        var rowOffset = Origin.Y / LatticeCell.CellData.EdgeLength + 1;
        var colNumber = DrawRect.Width / LatticeCell.CellData.EdgeLength + 2;
        var rowNumber = DrawRect.Height / LatticeCell.CellData.EdgeLength + 2;
        var cellBrush = new Dictionary<Color, SolidBrush>();
        for (var i = 0; i < colNumber; i++)
        {
            for (var j = 0; j < rowNumber; j++)
            {
                var cell = new LatticeCell(Origin, new Coordinate(i - colOffset, j - rowOffset));
                var color = cell.TerrainPoint.GetTerrain().GetColor();
                if (!cellBrush.TryGetValue(color, out SolidBrush? brush))
                    brush = cellBrush[color] = new SolidBrush(color);
                Graphics?.FillRectangle(brush, cell.CenterRealRect);
            }
        }
    }
}
