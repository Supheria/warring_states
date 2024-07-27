using LocalUtilities.TypeGeneral;
using WarringStates.Client.Map;

namespace WarringStates.Client.Graph;

partial class GridDrawer
{
    public sealed class Cell
    {
        public static Coordinate GridOrigin { get; set; } = new();

        public Rectangle RealRect { get; } 

        public Rectangle PartRealRect { get; private set; } = new();

        public Color PartShading { get; private set; } = new();

        public Coordinate Site { get; } 

        public Directions Part { get; private set; }

        public Cell(Coordinate latticePoint)
        {
            RealRect = GetRealRect(latticePoint);
            Site = Atlas.SetPointWithin(latticePoint);
            SetPart(Directions.Center);
        }

        public Cell(Point realPoint) : this(RealPointToLatticePoint(realPoint))
        {
            SetPart(GetRealPointOnPart(realPoint));
        }

        public void SetPart(Directions part)
        {
            Part = part;
            PartRealRect = GetPartRealRect(part);
            PartShading = GetCellPartShading(part);
        }

        private static Coordinate RealPointToLatticePoint(Point realPoint)
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

        private static Rectangle GetRealRect(Coordinate latticePoint)
        {
            var sX = CellEdgeLength * latticePoint.X;
            var x = sX + GridOrigin.X;
            var sY = CellEdgeLength * latticePoint.Y;
            var y = sY + GridOrigin.Y;
            return new(x, y, CellEdgeLength, CellEdgeLength);
        }

        private Rectangle GetPartRealRect(Directions part)
        {
            var centerRealRect = new Rectangle(new(RealRect.Left + CellCenterPadding, RealRect.Top + CellCenterPadding), CellCenterSize);
            return part switch
            {
                Directions.Center => centerRealRect,
                Directions.Left => new Rectangle(RealRect.Left, centerRealRect.Top, CellCenterPadding, CellCenterSize.Height),
                Directions.Top => new Rectangle(centerRealRect.Left, RealRect.Top, CellCenterSize.Width, CellCenterPadding),
                Directions.Right => new Rectangle(centerRealRect.Right, centerRealRect.Top, CellCenterPadding, centerRealRect.Height),
                Directions.Bottom => new Rectangle(centerRealRect.Left, centerRealRect.Bottom, centerRealRect.Width, CellCenterPadding),
                Directions.LeftTop => new Rectangle(RealRect.Left, RealRect.Top, CellCenterPadding, CellCenterPadding),
                Directions.TopRight => new Rectangle(centerRealRect.Right, RealRect.Top, CellCenterPadding, CellCenterPadding),
                Directions.BottomRight => new Rectangle(centerRealRect.Right, centerRealRect.Bottom, CellCenterPadding, CellCenterPadding),
                Directions.LeftBottom => new Rectangle(RealRect.Left, centerRealRect.Bottom, CellCenterPadding, CellCenterPadding),
                _ => new()
            };
        }

        private Directions GetRealPointOnPart(Point realpoint)
        {
            if (GetPartRealRect(Directions.Center).Contains(realpoint))
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

        private Color GetCellPartShading(Directions direction)
        {
            if (CellPartShading.TryGetValue(Part, out var shading))
                return shading;
            return new();
        }

        private static Dictionary<Directions, Color> CellPartShading = new()
        {
            [Directions.Center] = Color.Orange,
            [Directions.Left] = Color.Gray,
            [Directions.Top] = Color.Gray,
            [Directions.Right] = Color.Gray,
            [Directions.Bottom] = Color.Gray,
            [Directions.LeftTop] = Color.Gray,
            [Directions.TopRight] = Color.Gray,
            [Directions.LeftBottom] = Color.Gray,
            [Directions.BottomRight] = Color.Gray,
        };
    }
}
