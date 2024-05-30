using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using OpenCvSharp;
using static WarringStates.Graph.LatticeGrid;

namespace WarringStates.Map;

partial class SourceLand
{
    static SolidBrush Brush { get; } = new(Color.Transparent);

    public int DrawCell(Graphics? g, Cell cell, Rectangle drawRect, Color backColor, ILand? lastLand)
    {
        if (g is null)
            return 0;
        var count = 0;
        var direction = Points[cell.TerrainPoint];
        if (direction is not Directions.Center)
        {
            Brush.Color = backColor;
            if (cell.RealRect.CutRectInRange(drawRect, out var r))
            {
                g?.FillRectangle(Brush, r.Value);
                count++;
            }
        }
        if (GetSourceLandCellRect(direction, cell).CutRectInRange(drawRect, out var rect))
        {
            Brush.Color = Colors[Type];
            g?.FillRectangle(Brush, rect.Value);
            count++;
        }
        return count;
    }

    public int DrawCell(Mat? mat, Cell cell, Rectangle drawRect, Color backColor, ILand? lastLand)
    {
        if (mat is null)
            return 0;
        var count = 0;
        var direction = Points[cell.TerrainPoint];
        if (direction is not Directions.Center)
        {
            if (cell.RealRect.CutRectInRange(drawRect, out var rc))
            {
                var color = new Scalar(backColor.B, backColor.G, backColor.R, backColor.A);
                var r = rc.Value;
                OpenCvSharp.Point[] vertices = [
                    new(r.Left, r.Top),
                    new(r.Right,r.Top),
                    new(r.Right, r.Bottom),
                    new(r.Left, r.Bottom),
                ];
                Cv2.FillPoly(mat, new List<OpenCvSharp.Point[]> { vertices }, color, LineTypes.Link8);
                count++;
            }
        }
        if (GetSourceLandCellRect(direction, cell).CutRectInRange(drawRect, out var rect))
        {
            var color = new Scalar(Colors[Type].B, Colors[Type].G, Colors[Type].R, Colors[Type].A);
            var r = rect.Value;
            OpenCvSharp.Point[] vertices = [
                new(r.Left, r.Top),
                new(r.Right,r.Top),
                new(r.Right, r.Bottom),
                new(r.Left, r.Bottom),
                ];
            Cv2.FillPoly(mat, new List<OpenCvSharp.Point[]> { vertices }, color, LineTypes.Link8);
            count++;
        }
        return count;
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
}
