using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using System.Drawing;
using WarringStates.Map;

namespace WarringStates.Graph;

public sealed class LatticeCell
{
    public static CellData CellData { get; set; } = new CellData().LoadFromSimpleScript();

    public Coordinate LatticePoint { get; }

    public Direction OnPart { get; }

    public Coordinate GridOrigin { get; }

    public LatticeCell(Coordinate gridOrigin, Point point)
    {
        GridOrigin = gridOrigin;
        LatticePoint = PointToLatticePoint(point);
        OnPart = PointOnCellPart(point);
    }

    public LatticeCell(Coordinate gridOrigin, Coordinate latticePoint)
    {
        GridOrigin = gridOrigin;
        LatticePoint = latticePoint;
        OnPart = Direction.None;
    }

    public Coordinate PointToLatticePoint(Point point)
    {
        var dX = point.X - GridOrigin.X;
        var dY = point.Y - GridOrigin.Y;
        var x = dX / CellData.EdgeLength;
        if (dX < 0)
            x--;
        var y = dY / CellData.EdgeLength;
        if (dY < 0)
            y--;
        return new(x, y);
    }

    public static int CenterPadding()
    {
        return (CellData.EdgeLength * CellData.CenterPaddingFactor).ToInt();
    }

    public Rectangle RealRect()
    {
        var sX = CellData.EdgeLength * LatticePoint.X;
        var x = sX + GridOrigin.X;
        //if (sX < GridOrigin.X)
        //    x -= CellData.EdgeLength * Terrain.Width;
        var sY = CellData.EdgeLength * LatticePoint.Y;
        var y = sY + GridOrigin.Y;
        //if (sY < GridOrigin.Y)
        //    y -= CellData.EdgeLength * Terrain.Height;
        return new(x, y, CellData.EdgeLength, CellData.EdgeLength);
    }

    public Rectangle CenterRealRect()
    {
        var cellRect = RealRect();
        var nodePadding = CenterPadding();
        return new(
            cellRect.Left + nodePadding, cellRect.Top + nodePadding,
            cellRect.Width - nodePadding * 2, cellRect.Height - nodePadding * 2);
    }

    private Direction PointOnCellPart(Point point)
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

    private Rectangle CellPartsRealRect(Direction part)
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
}
