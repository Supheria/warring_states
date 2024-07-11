using LocalUtilities.TypeGeneral;
using WarringStates.Client.Map;

namespace WarringStates.Client.Graph;

partial class LatticeGrid
{
    public sealed class Cell
    {
        public static Coordinate GridOrigin { get; set; } = new();

        public Coordinate LatticePoint { get; }

        public Rectangle RealRect { get; }

        public Rectangle CenterRealRect { get; }

        public Coordinate TerrainPoint { get; }

        public Cell(Coordinate latticePoint)
        {
            LatticePoint = latticePoint;
            RealRect = GetRealRect(LatticePoint);
            CenterRealRect = GetCenterRealRect(RealRect);
            TerrainPoint = latticePoint.SetPointWithinTerrainMap();
        }

        private static Rectangle GetRealRect(Coordinate latticePoint)
        {
            var sX = CellEdgeLength * latticePoint.X;
            var x = sX + GridOrigin.X;
            var sY = CellEdgeLength * latticePoint.Y;
            var y = sY + GridOrigin.Y;
            return new(x, y, CellEdgeLength, CellEdgeLength);
        }

        private static Rectangle GetCenterRealRect(Rectangle realRect)
        {
            return new(new(realRect.Left + CellCenterPadding, realRect.Top + CellCenterPadding), CellCenterSize);
        }

        public Rectangle GetPartRealRect(Directions direction)
        {
            return direction switch
            {
                Directions.Left => new Rectangle(RealRect.Left, CenterRealRect.Top, CellCenterPadding, CellCenterSize.Height),
                Directions.Top => new Rectangle(CenterRealRect.Left, RealRect.Top, CellCenterSize.Width, CellCenterPadding),
                Directions.Right => new Rectangle(CenterRealRect.Right, CenterRealRect.Top, CellCenterPadding, CenterRealRect.Height),
                Directions.Bottom => new Rectangle(CenterRealRect.Left, CenterRealRect.Bottom, CenterRealRect.Width, CellCenterPadding),
                Directions.LeftTop => new Rectangle(RealRect.Left, RealRect.Top, CellCenterPadding, CellCenterPadding),
                Directions.TopRight => new Rectangle(CenterRealRect.Right, RealRect.Top, CellCenterPadding, CellCenterPadding),
                Directions.BottomRight => new Rectangle(CenterRealRect.Right, CenterRealRect.Bottom, CellCenterPadding, CellCenterPadding),
                Directions.LeftBottom => new Rectangle(RealRect.Left, CenterRealRect.Bottom, CellCenterPadding, CellCenterPadding),
                _ => new()
            };
        }

        public Directions GetRealPointOnPart(Point realpoint)
        {
            if (CenterRealRect.Contains(realpoint))
                return Directions.Center;
            if (GetPartRealRect(Directions.Left).Contains(realpoint))
                return Directions.Left;
            if (GetPartRealRect(Directions.Top).Contains(realpoint))
                return Directions.Top;
            if (GetPartRealRect(Directions.Right).Contains(realpoint))
                return Directions.Right;
            if (GetPartRealRect(Directions.Bottom).Contains(realpoint))
                return Directions.Bottom;
            if (GetPartRealRect(Directions.LeftTop).Contains(realpoint))
                return Directions.LeftTop;
            if (GetPartRealRect(Directions.TopRight).Contains(realpoint))
                return Directions.TopRight;
            if (GetPartRealRect(Directions.BottomRight).Contains(realpoint))
                return Directions.BottomRight;
            if (GetPartRealRect(Directions.LeftBottom).Contains(realpoint))
                return Directions.LeftBottom;
            return Directions.None;
        }
    }
}
