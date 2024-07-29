using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Convert;
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
    static bool IsDrawingSelect { get; set; } = false;

    static bool IsWaiting { get; set; } = false;
    static bool IsWaitingSelect { get; set; } = false;

    //static Coordinate SelectCell { get; set; } = null;

    static Cell? LastSelectCell { get; set; } = null;

    public static void OffsetOrigin(Coordinate offset)
    {
        GridWidth = Atlas.Width * CellEdgeLength;
        GridHeight = Atlas.Height * CellEdgeLength;
        var x = (Origin.X + offset.X) % GridWidth;
        if (x < 0)
            x += GridWidth;
        var y = (Origin.Y + offset.Y) % GridHeight;
        if (y < 0)
            y += GridHeight;
        Origin = new(x, y);
        LocalEvents.TryBroadcast(LocalEvents.Graph.GridOriginReset);
    }

    public static async void RedrawAsync(int width, int height, Color backColor/*, Point? select*/)
    {
        if (width <= 0 || height <= 0)
            return;
        //if (select is not null)
        //    SelectCell = new(select.Value);
        if (IsDrawing)
        {
            IsWaiting = true;
            return;
        }
        IsDrawing = true;
        var source = new Bitmap(width, height);
        DrawRect = new(new(0, 0), source.Size);
        //LastSelectCell = null;
        await Task.Run(() => Redraw(source, backColor));
        var sendArgs = new GridRedrawArgs(source, DrawRect, Origin);
        LocalEvents.TryBroadcast(LocalEvents.Graph.GridRedraw, sendArgs);
        IsDrawing = false;
        if (IsWaiting)
        {
            RedrawAsync(width, height, backColor/*, select*/);
            IsWaiting = false;
        }
    }

    //public static void Redraw(int width, int height, Color backColor, Point? select)
    //{
    //    if (select is not null)
    //        SelectCell = new(select.Value);
    //    IsDrawing = true;
    //    var source = new Bitmap(width, height);
    //    DrawRect = new(new(0, 0), source.Size);
    //    LastSelectCell = null;
    //    Redraw(source, backColor);
    //    var sendArgs = new GridRedrawArgs(source, DrawRect, Origin);
    //    LocalEvents.TryBroadcast(LocalEvents.Graph.GridRedraw, sendArgs);
    //    IsDrawing = false;
    //}

    private static void Redraw(Image source, Color backColor)
    {
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
        if (LastSelectCell is not null)
            DrawSelect(source, backColor, LastSelectCell, null);
        DrawGuideLine(g);
    }

    public static async void RedrawSelectAsync(Image image, Color backColor, Point select)
    {
        if (IsDrawingSelect)
        {
            IsWaitingSelect = true;
            return;
        }
        var cell = new Cell(select);
        if (LastSelectCell is not null &&
            LastSelectCell.Site == cell.Site &&
            LastSelectCell.Part == cell.Part)
            return;
        IsDrawingSelect = true;
        var source = (Image)image.Clone();
        DrawRect = new(new(0, 0), source.Size);
        await Task.Run(() => DrawSelect(source, backColor, cell, LastSelectCell));
        LastSelectCell = cell;
        var sendArgs = new GridRedrawArgs(source, DrawRect, Origin);
        LocalEvents.TryBroadcast(LocalEvents.Graph.GridRedraw, sendArgs);
        IsDrawingSelect = false;
        if (IsWaiting)
        {
            RedrawSelectAsync(image, backColor, select);
            IsWaitingSelect = false;
        }
    }

    private static void DrawSelect(Image source, Color backColor, Cell selectCell, Cell? lastSelectCell)
    {
        LocalEvents.TryBroadcast(LocalEvents.Test.AddSingleInfo, new TestForm.StringInfo("select bounds", selectCell.GetBounds().ToArrayString()));
        using var g = Graphics.FromImage(source);
        if (lastSelectCell is not null)
        {
            g.FillRectangle(new SolidBrush(backColor), lastSelectCell.GetBounds());
            var lastLand = Atlas.GetLand(lastSelectCell.Site);
            DrawLand(g, lastLand, lastSelectCell);
        }
        if (!GeometryTool.CutRectInRange(selectCell.GetPartBounds(selectCell.Part), DrawRect, out var rect))
            return;
        var pSource = new PointBitmap((Bitmap)source);
        pSource.LockBits();
        for (var i = 0; i < rect.Width; i++)
        {
            for (var j = 0; j < rect.Height; j++)
            {
                var x = rect.Left + i;
                var y = rect.Top + j;
                var color = BitmapTool.GetMixedColor(pSource.GetPixel(x, y), selectCell.PartShading);
                pSource.SetPixel(x, y, color);
            }
        }
        pSource.UnlockBits();
    }

    private static void DrawLand(Graphics g, Land land, Cell cell)
    {
        if (land is SingleLand singleLand && singleLand.LandType is not SingleLandTypes.None)
        {
            if (GeometryTool.CutRectInRange(cell.GetPartBounds(Directions.Center), DrawRect, out var rect))
                g.FillRectangle(new SolidBrush(land.Color), rect);
        }
        else if (land is SourceLand sourceLand && sourceLand.LandType is not SourceLandTypes.None)
        {
            if (GeometryTool.CutRectInRange(GetSourceLandBounds(sourceLand.Direction, cell), DrawRect, out var rect))
                g.FillRectangle(new SolidBrush(land.Color), rect);
        }
        else
        {
            if (GeometryTool.CutRectInRange(cell.GetPartBounds(Directions.Center), DrawRect, out var rect))
                g.DrawRectangle(new Pen(land.Color), rect);
        }
    }

    private static Rectangle GetSourceLandBounds(Directions direction, Cell cell)
    {
        var bounds = cell.GetBounds();
        var centerBounds = cell.GetPartBounds(Directions.Center);
        return direction switch
        {
            Directions.LeftTop => new(new(centerBounds.Left, centerBounds.Top), CellCenterSizeAddOnePadding),
            Directions.Top => new(bounds.Left, centerBounds.Top, CellEdgeLength, CellCenterSizeAddOnePadding.Height),
            Directions.TopRight => new(new(bounds.Left, centerBounds.Top), CellCenterSizeAddOnePadding),
            Directions.Left => new(centerBounds.Left, bounds.Top, CellCenterSizeAddOnePadding.Width, CellEdgeLength),
            Directions.Center => bounds,
            Directions.Right => new(bounds.Left, bounds.Top, CellCenterSizeAddOnePadding.Width, CellEdgeLength),
            Directions.LeftBottom => new(new(centerBounds.Left, bounds.Top), CellCenterSizeAddOnePadding),
            Directions.Bottom => new(bounds.Left, bounds.Top, CellEdgeLength, CellCenterSizeAddOnePadding.Height),
            Directions.BottomRight => new(new(bounds.Left, bounds.Top), CellCenterSizeAddOnePadding),
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
