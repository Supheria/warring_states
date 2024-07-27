using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Graph;
using LocalUtilities.TypeToolKit.Mathematic;
using WarringStates.Client.Events;
using WarringStates.Client.Map;
using WarringStates.Client.UI;
using WarringStates.Map;
using static WarringStates.Client.Graph.GridDrawer;

namespace WarringStates.Client.Graph;

partial class GridDrawer
{
    static Rectangle DrawRect { get; set; }

    static bool IsDrawing { get; set; } = false;

    static bool IsWaiting { get; set; } = false;

    static Cell? SelectCell { get; set; } = null;

    static Cell? LastSelectCell { get; set; } = null;

    public static void OffsetOrigin(Coordinate offset)
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
        LocalEvents.TryBroadcast(LocalEvents.Graph.GridOriginReset);
    }

    public static async void RedrawAsync(int width, int height, Color backColor, Point? select)
    {
        if (width <= 0 || height <= 0)
            return;
        if (select is not null)
            SelectCell = new(select.Value);
        if (IsDrawing)
        {
            IsWaiting = true;
            return;
        }
        IsDrawing = true;
        var source = new Bitmap(width, height);
        DrawRect = new(new(0, 0), source.Size);
        await Task.Run(() => Redraw(source, backColor));
        LastSelectCell = null;
        var sendArgs = new GridRedrawArgs(source, DrawRect, Origin);
        LocalEvents.TryBroadcast(LocalEvents.Graph.GridRedraw, sendArgs);
        IsDrawing = false;
        if (IsWaiting)
        {
            RedrawAsync(width, height, backColor, select);
            IsWaiting = false;
        }
    }

    private static void Redraw(Image source, Color backColor)
    {
        Cell.GridOrigin = Origin;
        using var g = Graphics.FromImage(source);
        g.Clear(backColor);
        var size = new Size(DrawRect.Width / CellEdgeLength + 2, DrawRect.Height / CellEdgeLength + 2);
        var offset = new Coordinate(Origin.X / CellEdgeLength + 1, Origin.Y / CellEdgeLength + 1);
        for (var i = 0; i < size.Width; i++)
        {
            for (var j = 0; j < size.Height; j++)
            {
                var cell = new Cell(new(i - offset.X, j - offset.Y));
                var land = Atlas.GetLand(cell.Site);
                DrawLand(g, land, cell);
            }
        }
        DrawGuideLine(g);
        if (SelectCell is null)
            return;
        DrawSelect(source, backColor, SelectCell.PartRealRect, SelectCell.PartShading);
        LastSelectCell = SelectCell;
    }

    public static async void RedrawSelectAsync(Image image, Color backColor, Point select)
    {
        if (IsDrawing)
        {
            IsWaiting = true;
            return;
        }
        SelectCell = new Cell(select);
        if (LastSelectCell is not null &&
            LastSelectCell.Site == SelectCell.Site &&
            LastSelectCell.Part == SelectCell.Part)
            return;
        IsDrawing = true;
        var source = (Image)image.Clone();
        DrawRect = new(new(0, 0), source.Size);
        await Task.Run(() => DrawSelect(source, backColor, SelectCell.PartRealRect, SelectCell.PartShading));
        LastSelectCell = SelectCell;
        var sendArgs = new GridRedrawArgs(source, DrawRect, Origin);
        LocalEvents.TryBroadcast(LocalEvents.Graph.GridRedraw, sendArgs);
        IsDrawing = false;
        if (IsWaiting)
        {
            RedrawSelectAsync(image, backColor, select);
            IsWaiting = false;
        }
    }

    private static void DrawSelect(Image source, Color backColor, Rectangle rect, Color shading)
    {
        using var g = Graphics.FromImage(source);
        if (LastSelectCell is not null)
        {
            g.FillRectangle(new SolidBrush(backColor), LastSelectCell.RealRect);
            var lastLand = Atlas.GetLand(LastSelectCell.Site);
            LastSelectCell.SetPart(Directions.Center);
            DrawLand(g, lastLand, LastSelectCell);
        }
        if (!GeometryTool.CutRectInRange(rect, DrawRect, out var saveRect))
            return;
        rect = saveRect.Value;
        var pSource = new PointBitmap((Bitmap)source);
        pSource.LockBits();
        for (var i = 0; i < rect.Width; i++)
        {
            for (var j = 0; j < rect.Height; j++)
            {
                var x = rect.Left + i;
                var y = rect.Top + j;
                var color = BitmapTool.GetMixedColor(pSource.GetPixel(x, y), shading);
                pSource.SetPixel(x, y, color);
            }
        }
        pSource.UnlockBits();
    }

    private static void DrawLand(Graphics g, Land land, Cell cell)
    {
        if (land is SingleLand singleLand && singleLand.LandType is not SingleLandTypes.None)
        {
            if (GeometryTool.CutRectInRange(cell.PartRealRect, DrawRect, out var rect))
                g.FillRectangle(new SolidBrush(land.Color), rect.Value);
        }
        else if (land is SourceLand sourceLand && sourceLand.LandType is not SourceLandTypes.None)
        {
            if (GeometryTool.CutRectInRange(GetSourceLandCellRect(sourceLand.Direction, cell), DrawRect, out var rect))
                g.FillRectangle(new SolidBrush(land.Color), rect.Value);
        }
        else
        {
            if (GeometryTool.CutRectInRange(cell.PartRealRect, DrawRect, out var rect))
                g.DrawRectangle(new Pen(land.Color), rect.Value);
        }
    }

    private static Rectangle GetSourceLandCellRect(Directions direction, Cell cell)
    {
        return direction switch
        {
            Directions.LeftTop => new(new(cell.PartRealRect.Left, cell.PartRealRect.Top), CellCenterSizeAddOnePadding),
            Directions.Top => new(cell.RealRect.Left, cell.PartRealRect.Top, CellEdgeLength, CellCenterSizeAddOnePadding.Height),
            Directions.TopRight => new(new(cell.RealRect.Left, cell.PartRealRect.Top), CellCenterSizeAddOnePadding),
            Directions.Left => new(cell.PartRealRect.Left, cell.RealRect.Top, CellCenterSizeAddOnePadding.Width, CellEdgeLength),
            Directions.Center => cell.RealRect,
            Directions.Right => new(cell.RealRect.Left, cell.RealRect.Top, CellCenterSizeAddOnePadding.Width, CellEdgeLength),
            Directions.LeftBottom => new(new(cell.PartRealRect.Left, cell.RealRect.Top), CellCenterSizeAddOnePadding),
            Directions.Bottom => new(cell.RealRect.Left, cell.RealRect.Top, CellEdgeLength, CellCenterSizeAddOnePadding.Height),
            Directions.BottomRight => new(new(cell.RealRect.Left, cell.RealRect.Top), CellCenterSizeAddOnePadding),
            _ => new()
        };
    }

    private static void DrawGuideLine(Graphics g)
    {
        using var pen = new Pen(GridData.GuideLineColor, GridData.GuideLineWidth);
        g.DrawLine(pen, new(Origin.X, DrawRect.Top), new(Origin.X, DrawRect.Bottom));
        g.DrawLine(pen, new(DrawRect.Left, Origin.Y), new(DrawRect.Right, Origin.Y));
    }
}
