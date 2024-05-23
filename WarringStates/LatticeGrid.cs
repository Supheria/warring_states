using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;

namespace WarringStates;

public static class LatticeGrid
{
    public static GridData GridData { get; set; } = new GridData().LoadFromSimpleScript();

    static Rectangle DrawRect { get; set; }

    static Graphics? Graphics { get; set; }

    static public int OriginX { get; set; }

    static public int OriginY { get; set; }

    static Rectangle LastDrawRect { get; set; } = new();

    static Point LastOrigin { get; set; }

    static Dictionary<Coordinate, Color> LastCellColor { get; set; } = [];

    static Rectangle LastGuideLineRectHorizon { get; set; } = new();

    static Rectangle LastGuideLineRectVertical { get; set; } = new();

    static Color BackColor { get; set; }

    public static void DrawLatticeGrid(this Image image, Rectangle drawRect, Color backColor)
    {
        Graphics = Graphics.FromImage(image);
        DrawRect = drawRect;
        BackColor = backColor;
        DrawLatticeCells();
        DrawGuideLine();
        Graphics.Flush();
        Graphics.Dispose();
        LastOrigin = new(OriginX, OriginY);
        LastDrawRect = drawRect;
    }

    private static void DrawGuideLine()
    {
        var brush = new SolidBrush(GridData.GuideLineColor);
        var brushClear = new SolidBrush(BackColor);
        var lineRectVertical = GetCrossLineRect(new(OriginX, DrawRect.Top), new(OriginX, DrawRect.Bottom), GridData.GuideLineWidth);
        if (lineRectVertical != LastGuideLineRectVertical)
        {
            Graphics?.FillRectangle(brush, lineRectVertical);
            Graphics?.FillRectangle(brushClear, LastGuideLineRectVertical);
            LastGuideLineRectVertical = lineRectVertical;
        }
        var lineRectHorizon = GetCrossLineRect(new(DrawRect.Left, OriginY), new(DrawRect.Right, OriginY), GridData.GuideLineWidth);
        if (lineRectHorizon != LastGuideLineRectHorizon)
        {
            Graphics?.FillRectangle(brush, lineRectHorizon);
            Graphics?.FillRectangle(brushClear, LastGuideLineRectHorizon);
            LastGuideLineRectHorizon = lineRectHorizon;
        }
    }

    /// <summary>
    /// 绘制循环格元（格元左上角坐标与栅格坐标系中心偏移量近似投射在一个格元大小范围内）
    /// </summary>
    /// <param name="g"></param>
    private static void DrawLatticeCells()
    {
        var edgeLength = LatticeCell.CellData.EdgeLength;
        var dX = DrawRect.X - OriginX;
        var dY = DrawRect.Y - OriginY;
        var colOffset = dX / edgeLength - (dX < 0 ? 1 : 0);
        var rowOffset = dY / edgeLength - (dY < 0 ? 1 : 0);
        var colNumber = DrawRect.Width / edgeLength + (dX == 0 ? 0 : 2);
        var rowNumber = DrawRect.Height / edgeLength + (dY == 0 ? 0 : 2);
        dX = OriginX - LastOrigin.X;
        dY = OriginY - LastOrigin.Y;
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
            if (cell.CenterRealRect().CutRectInRange(DrawRect, out var rect))
            {
                if (!cellBrush.TryGetValue(color, out SolidBrush? brush))
                    brush = cellBrush[color] = new SolidBrush(color);
                Graphics?.FillRectangle(brush, rect.Value);
            }
        }
    }

    public static Rectangle GetCrossLineRect(Coordinate p1, Coordinate p2, double lineWidth)
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
