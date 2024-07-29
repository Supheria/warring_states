using LocalUtilities.TypeGeneral;
using WarringStates.Client.Map;

namespace WarringStates.Client.Graph;

partial class GridDrawer
{
    public sealed class Cell
    {
        private Coordinate GridPoint { get; }

        public Coordinate Site { get; }

        public Directions Part { get; } = Directions.Center;

        public Color PartShading { get; } = new();

        public Cell(Coordinate gridPoint)
        {
            GridPoint = gridPoint;
            Site = Atlas.SetPointWithin(gridPoint);
            Part = Directions.None;
            PartShading = new();
        }

        public Cell(Point realPoint)
        {
            GridPoint = RealPointToGridPoint(realPoint);
            Site = Atlas.SetPointWithin(GridPoint);
            Part = GetRealPointOnPart(realPoint);
            PartShading = Part switch
            {
                Directions.Center => Color.Orange,
                _ => Color.Gray,
            };
        }

        private static Coordinate RealPointToGridPoint(Point realPoint)
        {
            var dX = realPoint.X - Origin.X;
            var x = dX / CellEdgeLength;
            if (dX < 0)
                x--;
            var dY = realPoint.Y - Origin.Y;
            var y = dY / CellEdgeLength;
            if (dY < 0)
                y--;
            return new(x, y);
        }

        private (int, int) GridPointToRealLeftTop()
        {
            var x = (CellEdgeLength * GridPoint.X + Origin.X) % GridWidth;
            if (x < -CellEdgeLength)
                x += GridWidth;
            else if (x > GridWidth - CellEdgeLength)
                x -= GridWidth;
            var y = (CellEdgeLength * GridPoint.Y + Origin.Y) % GridHeight;
            if (y < -CellEdgeLength) 
                y += GridHeight;
            else if (y > GridHeight - CellEdgeLength)
                y -= GridHeight;
            return new(x, y);
        }

        public Rectangle GetBounds()
        {
            var (left, top) = GridPointToRealLeftTop();
            return new(left, top, CellEdgeLength, CellEdgeLength);
        }

        public Rectangle GetPartBounds(Directions part)
        {
            var (left, top) = GridPointToRealLeftTop();
            var centerRealRect = new Rectangle(new(left + CellCenterPadding, top + CellCenterPadding), CellCenterSize);
            return part switch
            {
                Directions.Center => centerRealRect,
                Directions.Left => new Rectangle(left, centerRealRect.Top, CellCenterPadding, CellCenterSize.Height),
                Directions.Top => new Rectangle(centerRealRect.Left, top, CellCenterSize.Width, CellCenterPadding),
                Directions.Right => new Rectangle(centerRealRect.Right, centerRealRect.Top, CellCenterPadding, centerRealRect.Height),
                Directions.Bottom => new Rectangle(centerRealRect.Left, centerRealRect.Bottom, centerRealRect.Width, CellCenterPadding),
                Directions.LeftTop => new Rectangle(left, top, CellCenterPadding, CellCenterPadding),
                Directions.TopRight => new Rectangle(centerRealRect.Right, top, CellCenterPadding, CellCenterPadding),
                Directions.BottomRight => new Rectangle(centerRealRect.Right, centerRealRect.Bottom, CellCenterPadding, CellCenterPadding),
                Directions.LeftBottom => new Rectangle(left, centerRealRect.Bottom, CellCenterPadding, CellCenterPadding),
                _ => new()
            };
        }

        private Directions GetRealPointOnPart(Point realpoint)
        {
            if (GetPartBounds(Directions.Center).Contains(realpoint))
                return Directions.Center;
            if (GetPartBounds(Directions.Left).Contains(realpoint))
                return Directions.Left;
            if (GetPartBounds(Directions.Top).Contains(realpoint))
                return Directions.Top;
            if (GetPartBounds(Directions.Right).Contains(realpoint))
                return Directions.Right;
            if (GetPartBounds(Directions.Bottom).Contains(realpoint))
                return Directions.Bottom;
            if (GetPartBounds(Directions.LeftTop).Contains(realpoint))
                return Directions.LeftTop;
            if (GetPartBounds(Directions.TopRight).Contains(realpoint))
                return Directions.TopRight;
            if (GetPartBounds(Directions.BottomRight).Contains(realpoint))
                return Directions.BottomRight;
            if (GetPartBounds(Directions.LeftBottom).Contains(realpoint))
                return Directions.LeftBottom;
            return Directions.None;
        }
    }
}
