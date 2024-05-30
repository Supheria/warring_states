using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using WarringStates.Map.Terrain;
using static WarringStates.Graph.LatticeGrid;

namespace WarringStates.Terrain;

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
