using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;

namespace WarringStates;

public sealed class LatticeCell
{
    public static CellData CellData { get; set; } = new CellData().LoadFromSimpleScript();

    public Coordinate LatticedPoint { get; set; }

    public static int CenterPadding()
    {
        return (CellData.EdgeLength * CellData.CenterPaddingFactor).ToInt();
    }

    public Rectangle RealRect(LatticeGrid grid)
    {
        return new(
        CellData.EdgeLength * LatticedPoint.X + grid.Origin.X,
        CellData.EdgeLength * LatticedPoint.Y + grid.Origin.Y,
        CellData.EdgeLength, CellData.EdgeLength
        );
    }

    public Rectangle CenterRealRect(LatticeGrid grid)
    {
        var cellRect = RealRect(grid);
        var nodePadding = CenterPadding();
        return new(
            cellRect.Left + nodePadding, cellRect.Top + nodePadding,
            cellRect.Width - nodePadding * 2, cellRect.Height - nodePadding * 2);
    }

    public LatticeCell() : this(new Coordinate())
    {

    }

    public LatticeCell(Coordinate latticedPoint)
    {
        LatticedPoint = latticedPoint;
    }
    /// <summary>
    /// 使用真实坐标创建格元
    /// </summary>
    /// <param name="realPoint"></param>
    public LatticeCell(Point realPoint, LatticeGrid grid)
    {
        var widthDiff = realPoint.X - grid.Origin.X;
        var heightDiff = realPoint.Y - grid.Origin.Y;
        var col = widthDiff / CellData.EdgeLength;
        var raw = heightDiff / CellData.EdgeLength;
        if (widthDiff < 0) { col--; }
        if (heightDiff < 0) { raw--; }
        LatticedPoint = new(col, raw);
    }

    /// <summary>
    /// 格元各个部分的真实坐标矩形
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public Rectangle CellPartsRealRect(Direction part, LatticeGrid grid)
    {
        var cellRect = RealRect(grid);
        var centerPadding = CenterPadding();
        var centerRect = CenterRealRect(grid);
        return part switch
        {
            Direction.Center => centerRect,
            Direction.Left => new(cellRect.Left, centerRect.Top, centerPadding, centerRect.Height),
            Direction.Top => new(centerRect.Left, cellRect.Top, centerRect.Width, centerPadding),
            Direction.Right => new(centerRect.Right, centerRect.Top, centerPadding, centerRect.Height),
            Direction.Bottom => new(centerRect.Left, centerRect.Bottom, centerRect.Width, centerPadding),
            Direction.LeftTop => new(cellRect.Left, cellRect.Top, centerPadding, centerPadding),
            Direction.TopRight => new(centerRect.Right, cellRect.Top, centerPadding, centerPadding),
            Direction.BottomRight => new(centerRect.Right, centerRect.Bottom, centerPadding, centerPadding),
            Direction.LeftBottom => new(cellRect.Left, centerRect.Bottom, centerPadding, centerPadding),
            _ => Rectangle.Empty,
        };
    }
    /// <summary>
    /// 获取坐标在格元上所处的部分
    /// </summary>
    /// <param name="point">坐标</param>
    /// <returns></returns>
    public Direction PointOnCellPart(Point point, LatticeGrid grid)
    {
        if (CellPartsRealRect(Direction.Center, grid).Contains(point))
            return Direction.Center;
        if (CellPartsRealRect(Direction.Left, grid).Contains(point))
            return Direction.Left;
        if (CellPartsRealRect(Direction.Top, grid).Contains(point))
            return Direction.Top;
        if (CellPartsRealRect(Direction.Right, grid).Contains(point))
            return Direction.Right;
        if (CellPartsRealRect(Direction.Bottom, grid).Contains(point))
            return Direction.Bottom;
        if (CellPartsRealRect(Direction.LeftTop, grid).Contains(point))
            return Direction.LeftTop;
        if (CellPartsRealRect(Direction.TopRight, grid).Contains(point))
            return Direction.TopRight;
        if (CellPartsRealRect(Direction.BottomRight, grid).Contains(point))
            return Direction.BottomRight;
        if (CellPartsRealRect(Direction.LeftBottom, grid).Contains(point))
            return Direction.LeftBottom;
        return Direction.None;
    }
}
