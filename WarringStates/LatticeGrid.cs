using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using static System.Windows.Forms.AxHost;

namespace WarringStates;

public static class LatticeGrid
{
    public static GridData GridData { get; set; } = new GridData().LoadFromSimpleScript();

    static Rectangle DrawRect { get; set; }

    public static int OriginX { get; set; }

    public static int OriginY { get; set; }

    public static void DrawLatticeGrid(this Image image)
    {
        DrawRect = new(new(0, 0), image.Size);
        var g = Graphics.FromImage(image);
        DrawLatticeCells(g);
        //
        // draw guide line
        //
        g.DrawLine(GridData.GuidePen, new(OriginX, DrawRect.Top), new(OriginX, DrawRect.Bottom));
        g.DrawLine(GridData.GuidePen, new(DrawRect.Left, OriginY), new(DrawRect.Right, OriginY));
        g.Flush(); g.Dispose();
    }

    /// <summary>
    /// 绘制循环格元（格元左上角坐标与栅格坐标系中心偏移量近似投射在一个格元大小范围内）
    /// </summary>
    /// <param name="g"></param>
    private static void DrawLatticeCells(Graphics g)
    {
        //var dX = OriginX - DrawRect.X;
        //var startX = OriginX / LatticeCell.CellData.EdgeLength;
        //if (startX > 0)
        //    startX -= LatticeCell.CellData.EdgeLength;
        //var dY = OriginY - DrawRect.Y;
        //var startY = OriginY / LatticeCell.CellData.EdgeLength;
        //if (startY > 0) 
        //    startY -= LatticeCell.CellData.EdgeLength;
        //var colNumber = DrawRect.Width / LatticeCell.CellData.EdgeLength;
        //if (dX is not 0)
        //    colNumber++;
        //var rowNumber = DrawRect.Height / LatticeCell.CellData.EdgeLength;
        //if (dY is not 0)
        //    rowNumber++;
        //var cell = new LatticeCell();
        //cell.LatticedPoint.Col = startX;

        var cell = new LatticeCell();
        var cellRect = cell.RealRect();
        var dX = DrawRect.X - OriginX;
        var dY = DrawRect.Y - OriginY;
        var colOffset = dX / cellRect.Width - (dX < 0 ? 1 : 0);
        var rowOffset = dY / cellRect.Height - (dY < 0 ? 1 : 0);
        cell.LatticedPoint.Col = colOffset;
        cell.LatticedPoint.Row = rowOffset;
        var colNumber = DrawRect.Width / cellRect.Width + 2;
        var rowNumber = DrawRect.Height / cellRect.Height + 2;
        for (var i = 0; i < colNumber; i++)
        {
            //cell.LatticedPoint.Row = startY;
            cell.LatticedPoint.Row = rowOffset;
            for (var j = 0; j < rowNumber; j++)
            {
                //var cellRect = cell.RealRect();
                cellRect = cell.RealRect();
                //
                // draw cell
                //
                if (CrossLineWithin(
                    new(cellRect.Left, cellRect.Bottom),
                    new(cellRect.Right, cellRect.Bottom),
                    GridData.CellPen.Width,
                    out var p1, out var p2
                    ))
                    g.DrawLine(GridData.CellPen, p1, p2);
                if (CrossLineWithin(
                    new(cellRect.Right, cellRect.Top),
                    new(cellRect.Right, cellRect.Bottom),
                    GridData.CellPen.Width,
                    out p1, out p2
                    ))
                    g.DrawLine(GridData.CellPen, p1, p2);
                var saveAll = RectWithin(cell.CenterRealRect(), out var saveRect);
                //
                // draw center
                //
                if (saveRect is not null)
                {
                    if (saveAll)
                        g.DrawRectangle(GridData.NodePenLine, saveRect.Value);
                    else
                        g.DrawRectangle(GridData.NodePenDash, saveRect.Value);
                }
                cell.LatticedPoint.Row++;
            }
            cell.LatticedPoint.Col++;
        }
    }

    //private void DrawCell(LatticeCell cell)
    //{
    //    var dX = 
    //}

    /// <summary>
    /// 获取给定的矩形在栅格绘图区域内的矩形
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="saveRect"></param>
    /// <returns>完全在绘图区域内返回true，否则返回false</returns>
    public static bool RectWithin(Rectangle rect, out Rectangle? saveRect)
    {
        saveRect = null;
        bool saveAll = true;
        var left = rect.Left;
        var right = rect.Right;
        var top = rect.Top;
        var bottom = rect.Bottom;
        if (left < DrawRect.Left)
        {
            saveAll = false;
            if (right <= DrawRect.Left)
                return false;
            left = DrawRect.Left;
        }
        if (right > DrawRect.Right)
        {
            saveAll = false;
            if (left >= DrawRect.Right)
                return false;
            right = DrawRect.Right;
        }
        if (top < DrawRect.Top)
        {
            saveAll = false;
            if (bottom <= DrawRect.Top)
                return false;
            top = DrawRect.Top;
        }
        if (bottom > DrawRect.Bottom)
        {
            saveAll = false;
            if (top >= DrawRect.Bottom)
                return false;
            bottom = DrawRect.Bottom;
        }
        saveRect = new(left, top, right - left, bottom - top);
        return saveAll;
    }

    /// <summary>
    /// 获取给定横纵直线在栅格绘图区域内的矩形
    /// </summary>
    /// <param name="p1">直线的端点</param>
    /// <param name="p2">直线的另一端点</param>
    /// <param name="lineWidth">直线的宽度</param>
    /// <param name="endMin"></param>
    /// <param name="endMax"></param>
    /// <returns></returns>
    public static bool CrossLineWithin(PointF p1, PointF p2, float lineWidth, out PointF endMin, out PointF endMax)
    {
        endMin = endMax = PointF.Empty;
        var halfLineWidth = (lineWidth / 2);
        //
        // horizontal line
        //
        if (DoubleEx.ApproxEqualTo(p1.Y, p2.Y))
        {
            if (!CrossLineWithin(
                p1.Y,
                DrawRect.Top, DrawRect.Bottom,
                (p1.X + halfLineWidth, p2.X + halfLineWidth),
                DrawRect.Left, DrawRect.Right,
                out var xMin, out var xMax
                ))
                return false;
            endMin = new((float)xMin, p1.Y);
            endMax = new((float)xMax, p1.Y);
        }
        //
        // vetical line
        //
        else
        {
            if (!CrossLineWithin(
                p1.X,
                DrawRect.Left, DrawRect.Right,
                (p1.Y + halfLineWidth, p2.Y + halfLineWidth),
                DrawRect.Top, DrawRect.Bottom,
                out var yMin, out var yMax
                ))
                return false;
            endMin = new(p1.X, (float)yMin);
            endMax = new(p1.X, (float)yMax);
        }
        return true;
    }

    private static bool CrossLineWithin(double theSame, double theSameLimitMin, double theSameLimitMax, (double, double) ends, double endLimitMin, double endLimitMax, out double endMin, out double endMax)
    {
        endMin = endMax = 0;
        if (ends.Item1.ApproxEqualTo(ends.Item2) ||
            theSame.ApproxLessThan(theSameLimitMin) ||
            theSame.ApproxEqualTo(theSameLimitMax))
            return false;
        endMin = Math.Min(ends.Item1, ends.Item2);
        endMax = Math.Max(ends.Item1, ends.Item2);
        if (endMin.ApproxGreaterThanOrEqualTo(endLimitMax))
            return false;
        if (endMax.ApproxLessThanOrEqualTo(endLimitMin))
            return false;
        endMin = endMin.ApproxLessThan(endLimitMin) ? endLimitMin : endMin;
        endMax = endMax.ApproxGreaterThan(endLimitMax) ? endLimitMax : endMax;
        return true;
    }
}
