using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Graph;
using LocalUtilities.TypeToolKit.Mathematic;
using System.Collections.Generic;
using WarringStates.Events;
using WarringStates.Graph;
using WarringStates.Map;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace WarringStates.UI.Component;

public partial class OverviewDisplayer : Displayer
{
    bool FullScreen { get; set; } = false;

    Bitmap? OverviewCache { get; set; }

    (double Width, double Height) FocusScaleRatio { get; set; }

    Rectangle FocusRect { get; set; }

    List<Rectangle> LastFocusOnRects { get; } = [];

    Pen FocusPen { get; set; } = new(Color.Red);

    List<Rectangle> FocusRects { get; } = [];

    public OverviewDisplayer()
    {
        AddOperations();
    }

    public void EnableListener()
    {
        LocalEvents.Hub.AddListener<GameDisplayerUpdatedArgs>(LocalEvents.UserInterface.GameDisplayerUpdate, SetBounds);
        LocalEvents.Hub.AddListener<GridUpdatedArgs>(LocalEvents.Graph.GridUpdate, Relocate);
    }

    private void SetBounds(GameDisplayerUpdatedArgs args)
    {
        if (Atlas.Width is 0 || Atlas.Height is 0)
            Size = new();
        else
        {
            var size = FullScreen ? args.DisplayRect.Size : new((int)(args.DisplayRect.Width * 0.25), (int)(args.DisplayRect.Height * 0.25));
            Size = Atlas.Size.ScaleSizeOnRatio(size);
            Location = FullScreen ? new((args.DisplayRect.Size - Size) / 2) : new(args.DisplayRect.Right - Width, args.DisplayRect.Top);
        }
    }

    private void Relocate(GridUpdatedArgs args)
    {
        if (Width is 0 || Height is 0)
            return;
        Relocate();
        RelocateOverview();
        RelocateFocus(args.DrawRect, args.Origin);
        Invalidate();
    }

    private void RelocateOverview()
    {
        if (OverviewCache is not null && Size == OverviewCache.Size)
        {
            OverviewCache.TemplateDrawPartsOn((Bitmap)Image, LastFocusOnRects, true);
            return;
        }
        OverviewCache?.Dispose();
        var widthUnit = (Height / (double)Atlas.Height).ToRoundInt();
        if (widthUnit is 0)
            widthUnit = 1;
        var heightUnit = (Width / (double)Atlas.Width).ToRoundInt();
        if (heightUnit is 0)
            heightUnit = 1;
        OverviewCache = new(Atlas.Width * widthUnit, Atlas.Height * heightUnit);
        var pOverview = new PointBitmap(OverviewCache);
        pOverview.LockBits();
        for (int i = 0; i < Atlas.Width; i++)
        {
            for (int j = 0; j < Atlas.Height; j++)
            {
                var color = Atlas.GetLand(new(i, j)).Color;
                drawUnit(i, j, color);
            }
        }
        pOverview.UnlockBits();
        var scaled = OverviewCache.CopyToNewSize(Size);
        OverviewCache.Dispose();
        OverviewCache = scaled;
        OverviewCache.Save("OverviewCache.bmp");
        Image = OverviewCache.Clone() as Image;
        void drawUnit(int col, int row, Color color)
        {
            var dx = widthUnit * col;
            var dy = heightUnit * row;
            for (var x = 0; x < widthUnit; x++)
            {
                for (var y = 0; y < heightUnit; y++)
                {
                    pOverview.SetPixel(x + dx, y + dy, color);
                }
            }
        }
    }

    private void RelocateFocus(Rectangle drawRect, Coordinate origin)
    {
        var width = drawRect.Width / LatticeGrid.CellEdgeLength;
        var height = drawRect.Height / LatticeGrid.CellEdgeLength;
        var x = Atlas.Width - origin.X / LatticeGrid.CellEdgeLength;
        var y = Atlas.Height - origin.Y / LatticeGrid.CellEdgeLength;
        var widthRatio = Atlas.Width / (double)Width;
        var heightRatio = Atlas.Height / (double)Height;
        FocusScaleRatio = (widthRatio, heightRatio);
        FocusRect = new Rectangle((x / widthRatio).ToRoundInt(), (y / heightRatio).ToRoundInt(), (width / widthRatio).ToRoundInt(), (height / heightRatio).ToRoundInt());
        using var g = Graphics.FromImage(Image);
        FocusRects.Clear();
        FocusRects.AddRange(FocusRect.CutRectLoopRectsInRange(new(new(0, 0), Size)));
        FocusPen.Width = Math.Min(Width, Height) * 0.01f;
        LastFocusOnRects.Clear();
        foreach (var rect in FocusRects)
        {
            g.DrawRectangle(FocusPen, rect);
            foreach (var edge in rect.GetRectEdges())
            {
                if (edge.GetCrossLineRect(FocusPen.Width).CutRectInRange(new(new(0, 0), Size), out var r))
                    LastFocusOnRects.Add(r.Value);
            }
        }
    }
}
