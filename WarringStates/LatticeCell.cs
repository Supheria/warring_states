using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;

namespace WarringStates;

public sealed class LatticeCell
{
    public static CellData CellData { get; set; } = new CellData().LoadFromSimpleScript();

    public LatticePoint LatticedPoint { get; set; }

    public static int CenterPadding()
    {
        return (CellData.EdgeLength * CellData.CenterPaddingFactor).ToInt();
    }

    public Rectangle RealRect()
    {
        return new(
        CellData.EdgeLength * LatticedPoint.Col + LatticeGrid.OriginX,
        CellData.EdgeLength * LatticedPoint.Row + LatticeGrid.OriginY,
        CellData.EdgeLength, CellData.EdgeLength
        );
    }

    public Rectangle CenterRealRect()
    {
        var cellRect = RealRect();
        var nodePadding = CenterPadding();
        return new(
            cellRect.Left + nodePadding, cellRect.Top + nodePadding,
            cellRect.Width - nodePadding * 2, cellRect.Height - nodePadding * 2);
    }
    /// <summary>
    /// 格元栅格化坐标
    /// </summary>

    public LatticeCell() : this(new LatticePoint())
    {

    }

    public LatticeCell(LatticePoint latticedPoint)
    {
        LatticedPoint = latticedPoint;
    }
    /// <summary>
    /// 使用真实坐标创建格元
    /// </summary>
    /// <param name="realPoint"></param>
    public LatticeCell(Point realPoint)
    {
        var widthDiff = realPoint.X - LatticeGrid.OriginX;
        var heightDiff = realPoint.Y - LatticeGrid.OriginY;
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
    public Rectangle CellPartsRealRect(Direction part)
    {
        var cellRect = RealRect();
        var centerPadding = CenterPadding();
        var centerRect = CenterRealRect();
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
    public Direction PointOnCellPart(Point point)
    {
        if (CellPartsRealRect(Direction.Center).Contains(point))
            return Direction.Center;
        if (CellPartsRealRect(Direction.Left).Contains(point))
            return Direction.Left;
        if (CellPartsRealRect(Direction.Top).Contains(point))
            return Direction.Top;
        if (CellPartsRealRect(Direction.Right).Contains(point))
            return Direction.Right;
        if (CellPartsRealRect(Direction.Bottom).Contains(point))
            return Direction.Bottom;
        if (CellPartsRealRect(Direction.LeftTop).Contains(point))
            return Direction.LeftTop;
        if (CellPartsRealRect(Direction.TopRight).Contains(point))
            return Direction.TopRight;
        if (CellPartsRealRect(Direction.BottomRight).Contains(point))
            return Direction.BottomRight;
        if (CellPartsRealRect(Direction.LeftBottom).Contains(point))
            return Direction.LeftBottom;
        return Direction.None;
    }
}
