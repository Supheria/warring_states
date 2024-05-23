using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Graph;
using LocalUtilities.TypeToolKit.Mathematic;
using System.Diagnostics;

namespace WarringStates;

public static class LatticeGrid
{
    public static GridData GridData { get; set; } = new GridData().LoadFromSimpleScript();

    static Rectangle DrawRect { get; set; }

    static Image? ImageSource { get; set; } = null;

    public static int OriginX { get; set; }

    public static int OriginY { get; set; }

    static Point LastOrigin { get; set; }

    public static void DrawLatticeGrid(this Image image, Rectangle drawRect)
    {
        ImageSource = image;
        DrawRect = drawRect;
        DrawLatticeCells();
        //
        // draw guide line
        //
        var g = Graphics.FromImage(ImageSource);
        g.DrawLine(GridData.GuidePen, new(OriginX, DrawRect.Top), new(OriginX, DrawRect.Bottom));
        g.DrawLine(GridData.GuidePen, new(DrawRect.Left, OriginY), new(DrawRect.Right, OriginY));
        g.Flush(); 
        g.Dispose();
        LastOrigin = new(OriginX, OriginY);
    }

    static TerrainSettingForm testForm = new()
    {
        TopMost = true,
    };
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
        var pSource = new PointBitmap((Bitmap)ImageSource!);
        var stop = new Stopwatch();
        testForm.Show();
        stop.Start();
        pSource.LockBits();
        for (var i = 0; i < colNumber; i++)
        {
            for (var j = 0; j < rowNumber; j++)
            {
                DrawCell(new(colOffset + i, rowOffset + j), pSource);
            }
        }
        pSource.UnlockBits();
        stop.Stop();
        testForm.Text = ($"PointBitmap: {stop.ElapsedMilliseconds} ");
        stop.Restart();
        stop.Start();
        var g = Graphics.FromImage(ImageSource!);
        for (var i = 0; i < colNumber; i++)
        {
            for (var j = 0; j < rowNumber; j++)
            {
                DrawCell(new(colOffset + i, rowOffset + j), g);
            }
        }

        g.Flush();
        g.Dispose();
        stop.Stop();
        testForm.Text += ($"Graphics: {stop.ElapsedMilliseconds}");
    }

    private static void DrawCell(LatticePoint point, Graphics g)
    {
        var color = point
            .ToCoordinateInTerrainMap()
            .GetTerrain()
            .GetColor();
        var cell = new LatticeCell(point);
        var cellRect = cell.RealRect();
        //var g = Graphics.FromImage(source);
        //
        // draw border
        //
        var lineRect = GetCrossLineRect(new(cellRect.Left, cellRect.Bottom), new(cellRect.Right, cellRect.Bottom), GridData.BorderWidth);
        g.FillRectangle(GridData.BorderBrush, lineRect);
        lineRect = GetCrossLineRect(new(cellRect.Left, cellRect.Top), new(cellRect.Left, cellRect.Bottom), GridData.BorderWidth);
        g.FillRectangle(GridData.BorderBrush, lineRect);

        if (cell.CenterRealRect().CutRectInRange(DrawRect, out var orect))
            g.FillRectangle(new SolidBrush(color), orect.Value);
    }

    private static void DrawCell(LatticePoint point, PointBitmap pSource)
    {
        var color = point
            .ToCoordinateInTerrainMap()
            .GetTerrain()
            .GetColor();
        var cell = new LatticeCell(point);
        var cellRect = cell.RealRect();
        //var g = Graphics.FromImage(source);
        //
        // draw border
        //
        var lineRect = GetCrossLineRect(new(cellRect.Left, cellRect.Bottom), new(cellRect.Right, cellRect.Bottom), GridData.BorderWidth);
        for (var i = lineRect.Left; i < lineRect.Right; i++)
        {
            for (var j = lineRect.Top; j < lineRect.Bottom; j++)
            {
                pSource.SetPixel(i, j, GridData.BorderColor);
            }
        }
        lineRect = GetCrossLineRect(new(cellRect.Left, cellRect.Top), new(cellRect.Left, cellRect.Bottom), GridData.BorderWidth);
        for (var i = lineRect.Left; i < lineRect.Right; i++)
        {
            for (var j = lineRect.Top; j < lineRect.Bottom; j++)
            {
                pSource.SetPixel(i, j, GridData.BorderColor);
            }
        }
        if (cell.CenterRealRect().CutRectInRange(DrawRect, out var orect))
        {
            var rect = orect.Value;
            for (var i = rect.Left; i < rect.Right; i++)
            {
                for (var j = rect.Top; j < rect.Bottom; j++)
                {
                    pSource.SetPixel(i, j, color);
                }
            }
        }

        //
        // draw center
        //
        //var saveAll = RectWithin(cell.CenterRealRect(), out var saveRect);
        //if (saveRect is not null)
        //    g.FillRectangle(new SolidBrush(color), saveRect.Value);
        {
            //if (saveAll)
            //    Graphics.DrawRectangle(GridData.NodePenLine, saveRect.Value);
            //else
            //    Graphics.DrawRectangle(GridData.NodePenDash, saveRect.Value);

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
