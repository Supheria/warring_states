using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.EventProcess;
using LocalUtilities.TypeToolKit.Mathematic;
using System.Windows.Forms;

namespace WarringStates;

public class LatticeGrid : IEventListener
{
    public GridData GridData { get; set; } = new GridData().LoadFromSimpleScript();

    Rectangle DrawRect { get; set; }

    Graphics? Graphics { get; set; }

    public Coordinate Origin { get; set; } = new();

    Rectangle LastDrawRect { get; set; } = new();

    Coordinate LastOrigin { get; set; } = new();

    Dictionary<Coordinate, Color> LastCellColor { get; set; } = [];

    Rectangle LastGuideLineRectHorizon { get; set; } = new();

    Rectangle LastGuideLineRectVertical { get; set; } = new();

    Color BackColor { get; set; }

    public LatticeGrid()
    {
        EventManager.Instance.AddEvent(LocalEventId.ImageUpdate, this);
    }

    public void HandleEvent(int eventId, IEventArgument argument)
    {
        if (argument is not GridToUpdateEventArgument arg)
            return;
        if (eventId is LocalEventId.ImageUpdate)
            DrawLatticeGrid(arg);
    }

    private void DrawLatticeGrid(GridToUpdateEventArgument arg)
    {
        var x = Origin.X + arg.OriginOffset.X;
        var y = Origin.Y + arg.OriginOffset.Y;
        var edgeLength = LatticeCell.CellData.EdgeLength;
        var width = Terrain.Width * edgeLength;
        x = x < 0 ? (x % width) + width : x % width;
        var height = Terrain.Height * edgeLength;
        y = y < 0 ? (y % height) + height : y % height;
        Origin = new(x, y);
        DrawRect = arg.DrawRect;
        BackColor = arg.BackColor;
        Graphics = Graphics.FromImage(arg.Source);
        DrawLatticeCells();
        DrawGuideLine();
        Graphics.Flush();
        Graphics.Dispose();
        LastOrigin = Origin;
        LastDrawRect = DrawRect;
        EventManager.Instance.Dispatch(LocalEventId.GridUpdate, new GridUpdatedEventArgument(DrawRect, Origin));
    }

    private void DrawGuideLine()
    {
        var brush = new SolidBrush(GridData.GuideLineColor);
        var brushClear = new SolidBrush(BackColor);
        var lineRectVertical = GetCrossLineRect(new(Origin.X, DrawRect.Top), new(Origin.X, DrawRect.Bottom), GridData.GuideLineWidth);
        Graphics?.FillRectangle(brush, lineRectVertical);
        if (lineRectVertical != LastGuideLineRectVertical)
        {
            Graphics?.FillRectangle(brushClear, LastGuideLineRectVertical);
            LastGuideLineRectVertical = lineRectVertical;
        }
        var lineRectHorizon = GetCrossLineRect(new(DrawRect.Left, Origin.Y), new(DrawRect.Right, Origin.Y), GridData.GuideLineWidth);
        Graphics?.FillRectangle(brush, lineRectHorizon);
        if (lineRectHorizon != LastGuideLineRectHorizon)
        {
            Graphics?.FillRectangle(brushClear, LastGuideLineRectHorizon);
            LastGuideLineRectHorizon = lineRectHorizon;
        }
    }

    private void DrawLatticeCells()
    {
        var edgeLength = LatticeCell.CellData.EdgeLength;
        var dX = DrawRect.X - Origin.X;
        var dY = DrawRect.Y - Origin.Y;
        var colOffset = dX / edgeLength - (dX < 0 ? 1 : 0);
        var rowOffset = dY / edgeLength - (dY < 0 ? 1 : 0);
        var colNumber = DrawRect.Width / edgeLength + (dX == 0 ? 0 : 2);
        var rowNumber = DrawRect.Height / edgeLength + (dY == 0 ? 0 : 2);
        dX = Origin.X - LastOrigin.X;
        dY = Origin.Y - LastOrigin.Y;
        var cellBrush = new Dictionary<Color, SolidBrush>();
        if (DrawRect != LastDrawRect || DrawRect.Height > LastDrawRect.Height || dX % edgeLength != 0 || dY % edgeLength != 0)
        {
            Graphics?.Clear(BackColor);
            for (var i = 0; i < colNumber; i++)
            {
                for (var j = 0; j < rowNumber; j++)
                {
                    var point = new Coordinate(colOffset + i, rowOffset + j);
                    var color = point.ToCoordinateWithinTerrainMap().GetTerrain().GetColor();
                    drawCell(point, color);
                }
            }
            return;
        }
        dX /= edgeLength;
        dY /= edgeLength;
        for (var i = 0; i < colNumber; i++)
        {
            for (var j = 0; j < rowNumber; j++)
            {
                var point = new Coordinate(colOffset + i, rowOffset + j);
                var color = point.ToCoordinateWithinTerrainMap().GetTerrain().GetColor();
                var lastPoint = new Coordinate(point.X + dX, point.Y + dY);
                if (!LastCellColor.TryGetValue(lastPoint, out var lastColor) || color != lastColor)
                    drawCell(point, color);
                LastCellColor[point] = color;
            }
        }
        void drawCell(Coordinate point, Color color)
        {
            var cell = new LatticeCell(point);
            if (cell.CenterRealRect(this).CutRectInRange(DrawRect, out var rect))
            {
                if (!cellBrush.TryGetValue(color, out SolidBrush? brush))
                    brush = cellBrush[color] = new SolidBrush(color);
                Graphics?.FillRectangle(brush, rect.Value);
            }
        }
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
}
