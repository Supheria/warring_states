using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using WarringStates.Map;

namespace WarringStates.Graph;

partial class LatticeGrid
{
    private class Cell
    {
        public static Coordinate GridOrigin { get; set; } = new();

        int CenterPadding { get; set; }

        public Rectangle RealRect { get; }

        public Rectangle CenterRealRect { get; }

        public Coordinate TerrainPoint { get; }

        public Cell(Coordinate latticePoint)
        {
            RealRect = GetRealRect(latticePoint);
            CenterPadding = (CellData.EdgeLength * CellData.CenterPaddingFactor).ToInt();
            CenterRealRect = GetCenterRealRect(RealRect, CenterPadding);
            TerrainPoint = GetTerrainPoint(latticePoint);
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

        private static Rectangle GetRealRect(Coordinate latticePoint)
        {
            var sX = CellData.EdgeLength * latticePoint.X;
            var x = sX + GridOrigin.X;
            var sY = CellData.EdgeLength * latticePoint.Y;
            var y = sY + GridOrigin.Y;
            return new(x, y, CellData.EdgeLength, CellData.EdgeLength);
        }

        private static Rectangle GetCenterRealRect(Rectangle realRect, int centerPadding)
        {
            return new(
                realRect.Left + centerPadding, realRect.Top + centerPadding,
                realRect.Width - centerPadding * 2, realRect.Height - centerPadding * 2);
        }

        public Direction GetRealPointOnPart(Point realpoint)
        {
            if (CenterRealRect.Contains(realpoint))
                return Direction.Center;
            if (new Rectangle(RealRect.Left, CenterRealRect.Top, CenterPadding, CenterRealRect.Height).Contains(realpoint))
                return Direction.Left;
            if (new Rectangle(CenterRealRect.Left, RealRect.Top, CenterRealRect.Width, CenterPadding).Contains(realpoint))
                return Direction.Top;
            if (new Rectangle(CenterRealRect.Right, CenterRealRect.Top, CenterPadding, CenterRealRect.Height).Contains(realpoint))
                return Direction.Right;
            if (new Rectangle(CenterRealRect.Left, CenterRealRect.Bottom, CenterRealRect.Width, CenterPadding).Contains(realpoint))
                return Direction.Bottom;
            if (new Rectangle(RealRect.Left, RealRect.Top, CenterPadding, CenterPadding).Contains(realpoint))
                return Direction.LeftTop;
            if (new Rectangle(CenterRealRect.Right, RealRect.Top, CenterPadding, CenterPadding).Contains(realpoint))
                return Direction.TopRight;
            if (new Rectangle(CenterRealRect.Right, CenterRealRect.Bottom, CenterPadding, CenterPadding).Contains(realpoint))
                return Direction.BottomRight;
            if (new Rectangle(RealRect.Left, CenterRealRect.Bottom, CenterPadding, CenterPadding).Contains(realpoint))
                return Direction.LeftBottom;
            return Direction.None;
        }
    }
}
