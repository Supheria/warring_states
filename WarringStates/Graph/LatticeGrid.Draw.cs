using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeGeneral.Convert;
using LocalUtilities.TypeToolKit.Mathematic;
using WarringStates.Events;
using WarringStates.Map;
using WarringStates.UI;

namespace WarringStates.Graph;

partial class LatticeGrid
{
    Rectangle LastDrawRect { get; set; } = new();

    Coordinate OriginOffset { get; set; } = new();

    Rectangle[] LastGuideLineRects { get; } = new Rectangle[2];

    int LastCellEdgeLength { get; set; } = CellEdgeLength;

    Color BackColor { get; set; }

    SolidBrush BackBrush { get; } = new(Color.Transparent);

    Dictionary<Color, SolidBrush> CellBrush { get; } = [];

    public Size LatticeSize { get; set; } = new();

    public Size LatticeOffset { get; set; } = new();

    private void DrawGrid(GameImageUpdateArgs args)
    {
        var lastOrigin = Origin;
        ResetOrigin(args.OriginOffset);
        OriginOffset = Origin - lastOrigin;
        DrawRect = new(new(0, 0), args.Source.Size);
        Graphics = Graphics.FromImage(args.Source);
        BackColor = args.BackColor;
        BackBrush.Color = BackColor;
        DrawGrid();
        Graphics.Flush();
        Graphics.Dispose();
        LastDrawRect = DrawRect;
        LastCellEdgeLength = CellEdgeLength;
        LocalEvents.Hub.Broadcast(LocalEvents.Graph.GridUpdate, new GridUpdatedArgs(DrawRect, Origin));
    }

    private void ResetOrigin(Coordinate offset)
    {
        var width = Atlas.Width * CellEdgeLength;
        var x = (Origin.X + offset.X) % width;
        if (x < 0)
            x += width;
        var height = Atlas.Height * CellEdgeLength;
        var y = (Origin.Y + offset.Y) % height;
        if (y < 0)
            y += height;
        Origin = new(x, y);
    }

    private void DrawGrid()
    {
        Cell.GridOrigin = Origin;
        LatticeSize = new(DrawRect.Width / CellEdgeLength + 2, DrawRect.Height / CellEdgeLength + 2);
        LatticeOffset = new(Origin.X / CellEdgeLength + 1, Origin.Y / CellEdgeLength + 1);
        var count = 0;
        //
        // redraw all
        //
        if (DrawRect != LastDrawRect ||
            OriginOffset.X % CellEdgeLength != 0 || OriginOffset.Y % CellEdgeLength != 0 ||
            LastCellEdgeLength != CellEdgeLength)
        {
            Graphics?.Clear(BackColor);
            for (var i = 0; i < LatticeSize.Width; i++)
            {
                for (var j = 0; j < LatticeSize.Height; j++)
                {
                    var cell = new Cell(new(i - LatticeOffset.Width, j - LatticeOffset.Height));
                    var land = cell.TerrainPoint.GetLand();
                    //var rect = land is SourceLand ? cell.RealRect : cell.CenterRealRect;
                    Rectangle rect;
                    if (land is SourceLand sourceLand)
                    {
                        rect = sourceLand.Points[cell.TerrainPoint] switch
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
                    else
                        rect = cell.CenterRealRect;
                    if (!rect.CutRectInRange(DrawRect, out var r))
                        continue;
                    if (!CellBrush.TryGetValue(land.Color, out var brush))
                        brush = CellBrush[land.Color] = new SolidBrush(land.Color);
                    Graphics?.FillRectangle(brush, r.Value);
                    count++;
                }
            }
            LocalEvents.Hub.Broadcast(LocalEvents.Test.AddSingleInfo, new TestForm.TestInfo("redraw cell count (all)", count.ToString()));
            DrawGuideLine();
            return;
        }
        //
        // redraw changed only
        //
        GridData.GuideLineBrush.Color = BackColor;
        Graphics?.FillRectangle(GridData.GuideLineBrush, LastGuideLineRects[0]);
        Graphics?.FillRectangle(GridData.GuideLineBrush, LastGuideLineRects[1]);
        DrawLatticeCells();
        DrawGuideLine();
    }

    private void DrawGuideLine()
    {
        GridData.GuideLineBrush.Color = GridData.GuideLineColor;
        var lineRect = GetLineRect(new(Origin.X, DrawRect.Top), new(Origin.X, DrawRect.Bottom), GridData.GuideLineWidth);
        Graphics?.FillRectangle(GridData.GuideLineBrush, lineRect);
        LastGuideLineRects[0] = lineRect;
        lineRect = GetLineRect(new(DrawRect.Left, Origin.Y), new(DrawRect.Right, Origin.Y), GridData.GuideLineWidth);
        LastGuideLineRects[1] = lineRect;
        Graphics?.FillRectangle(GridData.GuideLineBrush, lineRect);
        static Rectangle GetLineRect(Coordinate p1, Coordinate p2, double lineWidth)
        {
            if (p1.Y == p2.Y)
            {
                var y = p1.Y - lineWidth / 2;
                var xMin = Math.Min(p1.X, p2.X);
                return new(xMin, (int)y, Math.Abs(p1.X - p2.X), (int)lineWidth);
            }
            else
            {
                var x = p1.X - lineWidth / 2;
                var yMin = Math.Min(p1.Y, p2.Y);
                return new((int)x, yMin, (int)lineWidth, Math.Abs(p1.Y - p2.Y));
            }
        }
    }

    private void DrawLatticeCells()
    {
        var count = 0;
        var dX = OriginOffset.X / CellEdgeLength;
        var dY = OriginOffset.Y / CellEdgeLength;
        for (var i = 0; i < LatticeSize.Width; i++)
        {
            for (var j = 0; j < LatticeSize.Height; j++)
            {
                var point = new Coordinate(i - LatticeOffset.Width, j - LatticeOffset.Height);
                var cell = new Cell(point);
                var land = cell.TerrainPoint.GetLand();
                var lastCell = new Cell(new(point.X + dX, point.Y + dY));
                var lastLand = lastCell.TerrainPoint.GetLand();
                Rectangle rect;
                if (land is SourceLand sourceLand)
                {
                    Graphics?.FillRectangle(BackBrush, cell.RealRect);
                    count++;
                    rect = sourceLand.Points[cell.TerrainPoint] switch
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
                else
                {
                    if (land.Type.Equals(lastLand.Type))
                        continue;
                    if (lastLand is SourceLand)
                    {
                        Graphics?.FillRectangle(BackBrush, cell.RealRect);
                        count++;
                    }
                    rect = cell.CenterRealRect;
                }
                if (!rect.CutRectInRange(DrawRect, out var r))
                    continue;
                if (!CellBrush.TryGetValue(land.Color, out var brush))
                    brush = CellBrush[land.Color] = new SolidBrush(land.Color);
                Graphics?.FillRectangle(brush, r.Value);
                count++;
            }
        }
        LocalEvents.Hub.Broadcast(LocalEvents.Test.AddSingleInfo, new TestForm.TestInfo("redraw cell count (part)", count.ToString()));
    }
}
