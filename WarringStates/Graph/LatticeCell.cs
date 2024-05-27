using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using WarringStates.Map;

namespace WarringStates.Graph;

public sealed class LatticeCell
{
    public static CellData CellData { get; set; } = new CellData().LoadFromSimpleScript();

    Coordinate GridOrigin { get; }

    Coordinate LatticePoint { get; }

    int CenterPadding { get; }

    public Coordinate TerrainPoint { get; }

    public Direction ReadPointOnPart { get; } = Direction.None;

    public Rectangle RealRect { get; }

    public Rectangle CenterRealRect { get; }

    public LatticeCell(Coordinate gridOrigin, Coordinate latticePoint)
    {
        GridOrigin = gridOrigin;
        LatticePoint = latticePoint;
        TerrainPoint = GetTerrainPoint(LatticePoint);
        CenterPadding = (CellData.EdgeLength * CellData.CenterPaddingFactor).ToInt();
        RealRect = GetRealRect(GridOrigin, LatticePoint);
        CenterRealRect = GetCenterRealRect(RealRect, CenterPadding);
    }

    public LatticeCell(Coordinate gridOrigin, Point realPoint)
    {
        GridOrigin = gridOrigin;
        LatticePoint = GetLatticePoint(GridOrigin, realPoint);
        TerrainPoint = GetTerrainPoint(LatticePoint);
        CenterPadding = (CellData.EdgeLength * CellData.CenterPaddingFactor).ToInt();
        RealRect = GetRealRect(GridOrigin, LatticePoint);
        CenterRealRect = GetCenterRealRect(RealRect, CenterPadding);
        ReadPointOnPart = GetRealPointOnPart(realPoint, RealRect, CenterRealRect, CenterPadding);
    }

    private static Coordinate GetLatticePoint(Coordinate gridOrigin, Point realPoint)
    {
        var dX = realPoint.X - gridOrigin.X;
        var x = dX / CellData.EdgeLength;
        if (dX < 0)
            x--;
        var dY = realPoint.Y - gridOrigin.Y;
        var y = dY / CellData.EdgeLength;
        if (dY < 0)
            y--;
        return new(x, y);
    }

    private static Coordinate GetTerrainPoint(Coordinate latticePoint)
    {
        if (Terrain.Width is 0 || Terrain.Height is 0)
            return new();
        var modX = latticePoint.X % Terrain.Width;
        var modY = latticePoint.Y % Terrain.Height;
        var x = modX < 0 ? Terrain.Width + modX : modX;
        var y = modY < 0 ? Terrain.Height + modY : modY;
        return new(x, y);
    }

    private static Rectangle GetRealRect(Coordinate gridOrigin, Coordinate latticePoint)
    {
        var sX = CellData.EdgeLength * latticePoint.X;
        var x = sX + gridOrigin.X;
        var sY = CellData.EdgeLength * latticePoint.Y;
        var y = sY + gridOrigin.Y;
        return new(x, y, CellData.EdgeLength, CellData.EdgeLength);
    }

    private static Rectangle GetCenterRealRect(Rectangle realRect, int centerPadding)
    {
        return new(
            realRect.Left + centerPadding, realRect.Top + centerPadding,
            realRect.Width - centerPadding * 2, realRect.Height - centerPadding * 2);
    }

    private static Direction GetRealPointOnPart(Point realpoint, Rectangle realRect, Rectangle centerRealRect, int centerPadding)
    {
        if (centerRealRect.Contains(realpoint))
            return Direction.Center;
        if (new Rectangle(realRect.Left, centerRealRect.Top, centerPadding, centerRealRect.Height).Contains(realpoint))
            return Direction.Left;
        if (new Rectangle(centerRealRect.Left, realRect.Top, centerRealRect.Width, centerPadding).Contains(realpoint))
            return Direction.Top;
        if (new Rectangle(centerRealRect.Right, centerRealRect.Top, centerPadding, centerRealRect.Height).Contains(realpoint))
            return Direction.Right;
        if (new Rectangle(centerRealRect.Left, centerRealRect.Bottom, centerRealRect.Width, centerPadding).Contains(realpoint))
            return Direction.Bottom;
        if (new Rectangle(realRect.Left, realRect.Top, centerPadding, centerPadding).Contains(realpoint))
            return Direction.LeftTop;
        if (new Rectangle(centerRealRect.Right, realRect.Top, centerPadding, centerPadding).Contains(realpoint))
            return Direction.TopRight;
        if (new Rectangle(centerRealRect.Right, centerRealRect.Bottom, centerPadding, centerPadding).Contains(realpoint))
            return Direction.BottomRight;
        if (new Rectangle(realRect.Left, centerRealRect.Bottom, centerPadding, centerPadding).Contains(realpoint))
            return Direction.LeftBottom;
        return Direction.None;
    }
}
