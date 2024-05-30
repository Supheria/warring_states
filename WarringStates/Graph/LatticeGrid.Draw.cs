using LocalUtilities.TypeGeneral;
using WarringStates.Events;
using WarringStates.Map;
using WarringStates.UI;

namespace WarringStates.Graph;

partial class LatticeGrid
{
    Image? Image { get; set; }

    Graphics? Graphics { get; set; }

    Rectangle LastDrawRect { get; set; } = new();

    Coordinate OriginOffset { get; set; } = new();

    Rectangle[] LastGuideLineRects { get; } = new Rectangle[2];

    int LastCellEdgeLength { get; set; } = CellEdgeLength;

    Color BackColor { get; set; }

    SolidBrush BackBrush { get; } = new(Color.Transparent);

    public Size LatticeSize { get; set; } = new();

    public Size LatticeOffset { get; set; } = new();

    private void OffsetOrigin(Coordinate offset)
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
        LocalEvents.Hub.Broadcast(LocalEvents.Graph.GridOriginReset);
    }

    private void SetOrigin(Coordinate origin)
    {
        var lastOrigin = Origin;
        Origin = origin;
        OriginOffset = Origin - lastOrigin;
        LocalEvents.Hub.Broadcast(LocalEvents.Graph.GridOriginReset);
    }

    private void UpdateImage(GridImageToUpdateArgs args)
    {
        DrawRect = new(new(0, 0), args.Source.Size);
        Image = args.Source;
        Graphics = Graphics.FromImage(Image);
        BackColor = args.BackColor;
        BackBrush.Color = BackColor;
        DrawGrid();
        Graphics.Flush();
        Graphics.Dispose();
        LastDrawRect = DrawRect;
        LastCellEdgeLength = CellEdgeLength;
        LocalEvents.Hub.Broadcast(LocalEvents.Graph.GridUpdated, new GridUpdatedArgs(DrawRect, Origin));
    }

    private void DrawGrid()
    {
        Cell.GridOrigin = Origin;
        LatticeSize = new(DrawRect.Width / CellEdgeLength + 2, DrawRect.Height / CellEdgeLength + 2);
        LatticeOffset = new(Origin.X / CellEdgeLength + 1, Origin.Y / CellEdgeLength + 1);
        //
        // redraw all
        //
        if (DrawRect != LastDrawRect ||
            OriginOffset.X % CellEdgeLength != 0 || OriginOffset.Y % CellEdgeLength != 0 ||
            LastCellEdgeLength != CellEdgeLength)
        {
            var count = 0;
            Graphics?.Clear(BackColor);
            for (var i = 0; i < LatticeSize.Width; i++)
            {
                for (var j = 0; j < LatticeSize.Height; j++)
                {
                    var cell = new Cell(new(i - LatticeOffset.Width, j - LatticeOffset.Height));
                    var land = cell.TerrainPoint.GetLand();
                    count += land.DrawCell(Graphics, cell, DrawRect, BackColor, null);
                }
            }
#if DEBUG
            LocalEvents.Hub.Broadcast(LocalEvents.Test.AddSingleInfo, new TestForm.StringInfo("redraw cell count (all)", count.ToString()));
#endif
        }
        //
        // redraw changed only
        //
        else
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
                    count += land.DrawCell(Graphics, cell, DrawRect, BackColor, lastLand);
                }
            }
#if DEBUG
            LocalEvents.Hub.Broadcast(LocalEvents.Test.AddSingleInfo, new TestForm.StringInfo("redraw cell count (part)", count.ToString()));
#endif
        }
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
}
