using LocalUtilities;
using LocalUtilities.General;
using WarringStates.Client.Map;
using WarringStates.Map;

namespace WarringStates.Client.Graph;

partial class GridDrawer
{
    private sealed class Cell
    {
        public Coordinate Site { get; }

        public Land Land { get; }

        public Directions PointOnPart { get; }

        public Color PartShading { get; }

        public Cell(Coordinate site)
        {
            Site = AtlasEx.SetPointWithin(site);
            Land = AtlasEx.GetLand(Site);
            PointOnPart = Directions.None;
            PartShading = new();
        }

        public Cell(Point realPoint)
        {
            Site = RealPointToSite(realPoint);
            Land = AtlasEx.GetLand(Site);
            PointOnPart = GetRealPointOnPart(realPoint);
            PartShading = GetPartShading(PointOnPart);
        }

        private static Coordinate RealPointToSite(Point realPoint)
        {
            var dX = realPoint.X - DrawOrigin.X;
            var x = dX / CellEdgeLength;
            if (dX < 0)
                x += AtlasEx.Width - 1;
            var dY = realPoint.Y - DrawOrigin.Y;
            var y = dY / CellEdgeLength;
            if (dY < 0)
                y += AtlasEx.Height - 1;
            return new(x, y);
        }

        private (int, int) GridPointToRealLeftTop()
        {
            var x = CellEdgeLength * Site.X + DrawOrigin.X;
            if (x < GridDrawRange.Left)
                x += GridSize.Width;
            else if (x > GridDrawRange.Right)
                x -= GridSize.Width;
            var y = CellEdgeLength * Site.Y + DrawOrigin.Y;
            if (y < GridDrawRange.Top)
                y += GridSize.Height;
            else if (y > GridDrawRange.Bottom)
                y -= GridSize.Height;
            return new(x, y);
        }

        public Rectangle GetBounds()
        {
            var (left, top) = GridPointToRealLeftTop();
            var bounds = new Rectangle(left, top, CellEdgeLength, CellEdgeLength);
            return GeometryTool.CutRectInRange(bounds, DrawRect);
        }

        public Rectangle GetPartBounds(Directions part)
        {
            var (left, top) = GridPointToRealLeftTop();
            var center = new Rectangle(new(left + CellCenterPadding, top + CellCenterPadding), CellCenterSize);
            var bounds = part switch
            {
                Directions.Center => center,
                Directions.Left => new(left, center.Top, CellCenterPadding, CellCenterSize.Height),
                Directions.Top => new(center.Left, top, CellCenterSize.Width, CellCenterPadding),
                Directions.Right => new(center.Right, center.Top, CellCenterPadding, center.Height),
                Directions.Bottom => new(center.Left, center.Bottom, center.Width, CellCenterPadding),
                Directions.LeftTop => new(left, top, CellCenterPadding, CellCenterPadding),
                Directions.TopRight => new(center.Right, top, CellCenterPadding, CellCenterPadding),
                Directions.LeftBottom => new(left, center.Bottom, CellCenterPadding, CellCenterPadding),
                Directions.BottomRight => new(center.Right, center.Bottom, CellCenterPadding, CellCenterPadding),
                _ => new()
            };
            return GeometryTool.CutRectInRange(bounds, DrawRect);
        }

        public Rectangle GetBoundsInDirection(Directions direction)
        {
            var (left, top) = GridPointToRealLeftTop();
            var bounds = new Rectangle(left, top, CellEdgeLength, CellEdgeLength);
            var center = new Rectangle(new(left + CellCenterPadding, top + CellCenterPadding), CellCenterSize);
            bounds = direction switch
            {
                Directions.Center => bounds,
                Directions.Left => new(center.Left, bounds.Top, CellCenterSizeAddOnePadding.Width, CellEdgeLength),
                Directions.Top => new(bounds.Left, center.Top, CellEdgeLength, CellCenterSizeAddOnePadding.Height),
                Directions.Right => new(bounds.Left, bounds.Top, CellCenterSizeAddOnePadding.Width, CellEdgeLength),
                Directions.Bottom => new(bounds.Left, bounds.Top, CellEdgeLength, CellCenterSizeAddOnePadding.Height),
                Directions.LeftTop => new(center.Location, CellCenterSizeAddOnePadding),
                Directions.TopRight => new(new(bounds.Left, center.Top), CellCenterSizeAddOnePadding),
                Directions.LeftBottom => new(new(center.Left, bounds.Top), CellCenterSizeAddOnePadding),
                Directions.BottomRight => new(new(bounds.Left, bounds.Top), CellCenterSizeAddOnePadding),
                _ => new()
            };
            return GeometryTool.CutRectInRange(bounds, DrawRect);
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

        public static Color GetPartShading(Directions part)
        {
            return part switch
            {
                Directions.Center => Color.Orange,
                _ => Color.Gray,
            };
        }
    }
}
