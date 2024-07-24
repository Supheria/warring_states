using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using WarringStates.Client.Events;
using WarringStates.Client.Map;
using WarringStates.Client.UI;
using WarringStates.Map.Terrain;

namespace WarringStates.Client.Graph;

partial class GridDrawer
{
    static Graphics? Graphics { get; set; }

    static Rectangle DrawRect { get; set; }

    static Rectangle LastDrawRect { get; set; } = new();

    static Coordinate OriginOffset { get; set; } = new();

    static Rectangle[] LastGuideLineRects { get; } = new Rectangle[2];

    static int LastCellEdgeLength { get; set; } = CellEdgeLength;

    static Color BackColor { get; set; }

    public static Size LatticeSize { get; set; } = new();

    public static Size LatticeOffset { get; set; } = new();

    static bool IsDrawing { get; set; } = false;

    public static void SetOrigin(Coordinate orgin)
    {
        var lastOrigin = Origin;
        Origin = orgin;
        OriginOffset = Origin - lastOrigin;
        LocalEvents.TryBroadcast(LocalEvents.Graph.GridOriginSet);
    }

    public static void OffsetOrigin(Coordinate offset)
    {
        var lastOrigin = Origin;
        var width = Atlas.Width * CellEdgeLength;
        var x = (Origin.X + offset.X) % width;
        if (x < 0)
            x += width;
        var height = Atlas.Height * CellEdgeLength;
        var y = (Origin.Y + offset.Y) % height;
        if (y < 0)
            y += height;
        Origin = new(x, y);
        OriginOffset = Origin - lastOrigin;
        LocalEvents.TryBroadcast(LocalEvents.Graph.GridOriginSet);
    }

    public static async Task<Bitmap?> RedrawAsync(int width, int height,/*Image source, */Color backColor)
    {
        if (IsDrawing)
            return null;
        IsDrawing = true;
        var source = new Bitmap(width, height);
        var task = Task.Run(() => Redraw(source, backColor));
        await task;
        var sendArgs = new GridRelocatedArgs(DrawRect, Origin);
        LocalEvents.TryBroadcast(LocalEvents.Graph.GridRedraw, sendArgs);
        IsDrawing = false;
        return source;
    }

    private static void Redraw(Image source, Color backColor)
    {
        DrawRect = new(new(0, 0), source.Size);
        Graphics?.Dispose();
        Graphics = Graphics.FromImage(source);
        BackColor = backColor;
        DrawGrid();
        Graphics.Flush();
        Graphics.Dispose();
        LastDrawRect = DrawRect;
        LastCellEdgeLength = CellEdgeLength;
    }

    private static void DrawGrid()
    {
        Cell.GridOrigin = Origin;
        LatticeSize = new(DrawRect.Width / CellEdgeLength + 2, DrawRect.Height / CellEdgeLength + 2);
        LatticeOffset = new(Origin.X / CellEdgeLength + 1, Origin.Y / CellEdgeLength + 1);
        //
        // redraw all
        //
        //if (DrawRect != LastDrawRect ||
        //    OriginOffset.X % CellEdgeLength != 0 || OriginOffset.Y % CellEdgeLength != 0 ||
        //    LastCellEdgeLength != CellEdgeLength)
        {
            var count = 0;
            Graphics?.Clear(BackColor);
            for (var i = 0; i < LatticeSize.Width; i++)
            {
                for (var j = 0; j < LatticeSize.Height; j++)
                {
                    var cell = new Cell(new(i - LatticeOffset.Width, j - LatticeOffset.Height));
                    var land = cell.TerrainPoint.GetLand();
                    if (land is SingleLand singleLand)
                        count += DrawSingleLand(cell, singleLand, null);
                    else if (land is SourceLand sourceLand)
                        count += DrawSourceLand(cell, sourceLand);
                }
            }
            return;
#if DEBUG
            //LocalEvents.TryBroadcast(LocalEvents.Test.AddSingleInfo, new TestForm.StringInfo("redraw cell count (all)", count.ToString()));
#endif
        }
        //
        // redraw changed only
        //
        //else
        {
            var count = 0;
            GridData.GuideLineBrush.Color = BackColor;
            Graphics?.FillRectangle(GridData.GuideLineBrush, LastGuideLineRects[0]);
            Graphics?.FillRectangle(GridData.GuideLineBrush, LastGuideLineRects[1]);
            var diff = OriginOffset / CellEdgeLength;
            for (var i = 0; i < LatticeSize.Width; i++)
            {
                for (var j = 0; j < LatticeSize.Height; j++)
                {
                    var cell = new Cell(new Coordinate(i - LatticeOffset.Width, j - LatticeOffset.Height));
                    var land = cell.TerrainPoint.GetLand();
                    var lastLand = new Cell(cell.LatticePoint + diff).TerrainPoint.GetLand();
                    if (land is SingleLand singleLand)
                        count += DrawSingleLand(cell, singleLand, lastLand);
                    else if (land is SourceLand sourceLand)
                        count += DrawSourceLand(cell, sourceLand);
                }
            }
#if DEBUG
            //LocalEvents.TryBroadcast(LocalEvents.Test.AddSingleInfo, new TestForm.StringInfo("redraw cell count (part)", count.ToString()));
#endif
        }
        DrawGuideLine();
    }

    private static int DrawSingleLand(Cell cell, SingleLand land, ILand? lastLand)
    {
        if (land.Type.Equals(lastLand?.Type))
            return 0;
        var count = 0;
        if (lastLand is SourceLand)
        {
            if (cell.RealRect.CutRectInRange(DrawRect, out var r))
            {
                Graphics?.FillRectangle(new SolidBrush(BackColor), r.Value);
                count++;
            }
        }
        if (cell.CenterRealRect.CutRectInRange(DrawRect, out var rect))
        {
            Graphics?.FillRectangle(new SolidBrush(land.Color), rect.Value);
            count++;
        }
        return count;
    }

    private static int DrawSourceLand(Cell cell, SourceLand land)
    {
        var count = 0;
        var direction = land[cell.TerrainPoint];
        if (direction is not Directions.Center)
        {
            if (cell.RealRect.CutRectInRange(DrawRect, out var r))
            {
                Graphics?.FillRectangle(new SolidBrush(BackColor), r.Value);
                count++;
            }
        }
        if (GetSourceLandCellRect(direction, cell).CutRectInRange(DrawRect, out var rect))
        {
            Graphics?.FillRectangle(new SolidBrush(land.Color), rect.Value);
            count++;
        }
        return count;
    }

    private static Rectangle GetSourceLandCellRect(Directions direction, Cell cell)
    {
        return direction switch
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

    private static void DrawGuideLine()
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
}
