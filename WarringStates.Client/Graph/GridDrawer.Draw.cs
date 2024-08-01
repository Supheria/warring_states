using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Graph;
using WarringStates.Client.Events;
using WarringStates.Client.Map;
using WarringStates.Map;

namespace WarringStates.Client.Graph;

partial class GridDrawer
{
    static Coordinate DrawOrigin { get; set; } = new();

    static Rectangle DrawRect { get; set; }

    static bool IsDrawing { get; set; } = false;

    static bool IsWaiting { get; set; } = false;

    static bool IsDrawingSelect { get; set; } = false;

    static bool IsWaitingSelect { get; set; } = false;

    static bool IsDrawingFocus { get; set; } = false;

    static bool IsWaitingFocus { get; set; } = false;

    static Cell? SelectCell { get; set; } = null;

    static Cell? FocusCell { get; set; } = null;

    static Color FocusColor { get; set; } = Color.Red;

    public static void OffsetOrigin(Coordinate offset)
    {
        GridSize = new(Atlas.Width * CellEdgeLength, Atlas.Height * CellEdgeLength);
        GridDrawRange = new(-CellEdgeLength, -CellEdgeLength, GridSize.Width, GridSize.Height);
        var x = (Origin.X + offset.X) % GridSize.Width;
        if (x < 0)
            x += GridSize.Width;
        var y = (Origin.Y + offset.Y) % GridSize.Height;
        if (y < 0)
            y += GridSize.Height;
        Origin = new(x, y);
        LocalEvents.TryBroadcast(LocalEvents.Graph.GridReset);
    }

    public static void Redraw(Size imageSize, Color backColor)
    {
        RedrawAsync(imageSize.Width, imageSize.Height, backColor);
    }

    public static void DrawSelect(Size imageSize, Color backColor, Point select)
    {
        var cell = new Cell(select);
        if (SelectCell?.Site == cell.Site && SelectCell?.PointOnPart == cell.PointOnPart)
            return;
        SelectCell = cell;
        RedrawAsync(imageSize.Width, imageSize.Height, backColor);
    }

    public static void DrawFocus(Size imageSize, Color backColor, Coordinate? select)
    {
        var cell = select is null ? null : new Cell(select);
        if (FocusCell?.Site == cell?.Site)
            return;
        FocusCell = cell;
        RedrawAsync(imageSize.Width, imageSize.Height, backColor);
    }

    private static async void RedrawAsync(int width, int height, Color backColor)
    {
        if (width <= 0 || height <= 0)
            return;
        if (IsDrawing)
        {
            IsWaiting = true;
            return;
        }
        IsDrawing = true;
        using var source = new Bitmap(width, height);
        DrawRect = new(new(0, 0), source.Size);
        DrawOrigin = Origin;
        await Task.Run(() => Redraw(source, backColor));
        var sendArgs = new GridRedrawArgs(source, DrawRect, Origin);
        LocalEvents.TryBroadcast(LocalEvents.Graph.GridRedraw, sendArgs);
        IsDrawing = false;
        if (IsWaiting)
        {
            RedrawAsync(width, height, backColor);
            IsWaiting = false;
        }
    }

    private static void Redraw(Image source, Color backColor)
    {
        using var g = Graphics.FromImage(source);
        DrawGrid(source, backColor);
        if (SelectCell is not null)
        {
            DrawCellShading(source, SelectCell.GetPartBounds(SelectCell.PointOnPart), SelectCell.PartShading);
        }
        if (FocusCell is not null)
        {
            var rect = FocusCell.GetBounds();
            if (!rect.IsEmpty)
            {
                var x = rect.Left + rect.Width / 2;
                var y = rect.Top + rect.Height / 2;
                DrawCrossLine(g, x, y, GridData.FocusLineColor, GridData.FocusLineWidth);
            }
        }
        DrawCrossLine(g, DrawOrigin.X, DrawOrigin.Y, GridData.GuideLineColor, GridData.GuideLineWidth);
    }

    private static void DrawGrid(Image source, Color backColor)
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
                DrawCell(g, cell);
            }
        }
    }

    private static void DrawCell(Graphics g, Cell cell)
    {
        if (cell.Land is SingleLand singleLand && singleLand.LandType is not SingleLandTypes.None)
            g.FillRectangle(new SolidBrush(cell.Land.Color), cell.GetPartBounds(Directions.Center));
        else if (cell.Land is SourceLand sourceLand && sourceLand.LandType is not SourceLandTypes.None)
            g.FillRectangle(new SolidBrush(cell.Land.Color), cell.GetBoundsInDirection(sourceLand.Direction));
        else
            g.DrawRectangle(new Pen(cell.Land.Color), cell.GetPartBounds(Directions.Center));
    }

    private static void DrawCellShading(Image source, Rectangle rect, Color shading)
    {
        var pSource = new PointBitmap((Bitmap)source);
        pSource.LockBits();
        for (var i = 0; i < rect.Width; i++)
        {
            for (var j = 0; j < rect.Height; j++)
            {
                var x = rect.Left + i;
                var y = rect.Top + j;
                pSource.SetPixel(x, y, BitmapTool.GetMixedColor(pSource.GetPixel(x, y), shading));
            }
        }
        pSource.UnlockBits();
    }

    private static void DrawCrossLine(Graphics g, int x, int y, Color color, float witdh)
    {
        using var pen = new Pen(color, witdh);
        g.DrawLine(pen, new(x, DrawRect.Top), new(x, DrawRect.Bottom));
        g.DrawLine(pen, new(DrawRect.Left, y), new(DrawRect.Right, y));
    }
}
