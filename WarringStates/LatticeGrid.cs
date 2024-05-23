using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Graph;
using LocalUtilities.TypeToolKit.Mathematic;

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
        DrawRect = drawRect;
        ImageSource = image;
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
        //for (var i = 0; i < colNumber; i++)
        //{
        //    for (var j = 0; j < rowNumber; j++)
        //    {
        //        DrawCell(new(new LatticePoint(colOffset + i, rowOffset + j)));
        //    }
        //}
        var imageParts = new Image[Environment.ProcessorCount];
        var widthSegment = ImageSource!.Width / imageParts.Length;
        widthSegment -= widthSegment % edgeLength;
        var widthSum = 0;
        for (int i = 0; i < imageParts.Length - 1; i++)
        {
            widthSum += widthSegment;
            imageParts[i] = new Bitmap(widthSegment, ImageSource.Height);
        }
        imageParts[imageParts.Length - 1] = new Bitmap(ImageSource.Width - widthSum, ImageSource.Height);
        //for (int i = 0; i < imageParts.Length; i++)
        Parallel.For(0, imageParts.Length, i =>
        {
            var image = imageParts[i];
            int xOffset = widthSegment * i;
            var startCol = xOffset / edgeLength + colOffset;
            var pSource = new PointBitmap((Bitmap)image);
            var colNumber = image.Width / edgeLength + (dX == 0 ? 0 : 2);
            var rowNumber = image.Height / edgeLength + (dY == 0 ? 0 : 2);
            pSource.LockBits();
            for (var col = 0; col < colNumber; col++)
            {
                for (var row = 0; row < rowNumber; row++)
                {
                    var color = new LatticePoint(startCol + col, rowOffset + row)
                        .ToCoordinateInTerrainMap()
                        .GetTerrain()
                        .GetColor();
                    var cell = new LatticeCell(new LatticePoint(colOffset + col, rowOffset + row));
                    DrawCell(cell, pSource, color);
                }
            }
            pSource.UnlockBits();
            //image.Save($@"test\{i}.bmp");
        });
        for (var i = 0; i < imageParts.Length; i++)
        {
            var image = (Bitmap)imageParts[i];
            image.TemplateDrawIntoRect((Bitmap)ImageSource, new(new(widthSegment * i, 0), image.Size), true);
            image.Dispose();
        }
    }

    private static void DrawCell(LatticeCell cell, PointBitmap pSource, Color color)
    {
        var coordinate = cell.LatticedPoint.ToCoordinateInTerrainMap();
        var terrain = coordinate.GetTerrain();
        //var color = terrain.GetColor();
        var cellRect = cell.RealRect();
        var bounds = new Rectangle(new(0, 0), pSource.Size);
        //var g = Graphics.FromImage(source);
        //
        // draw border
        //
        var lineRect = GetCrossLineRect(bounds, new(cellRect.Left, cellRect.Bottom), new(cellRect.Right, cellRect.Bottom), GridData.BorderWidth);
        for (var i = lineRect.Left; i < lineRect.Right; i++)
        {
            for (var j = lineRect.Top; j < lineRect.Bottom; j++)
            {
                pSource.SetPixel(i, j, GridData.BorderColor);
            }
        }
        lineRect = GetCrossLineRect(bounds, new(cellRect.Right, cellRect.Top), new(cellRect.Right, cellRect.Bottom), GridData.BorderWidth);
        for (var i = lineRect.Left; i < lineRect.Right; i++)
        {
            for (var j = lineRect.Top; j < lineRect.Bottom; j++)
            {
                pSource.SetPixel(i, j, GridData.BorderColor);
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

    public static Rectangle GetCrossLineRect(Rectangle bounds, Coordinate p1, Coordinate p2, double lineWidth)
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
        if (lineRect.CutRectInRange(bounds, out var result))
            return result.Value;
        return new();
    }
}
