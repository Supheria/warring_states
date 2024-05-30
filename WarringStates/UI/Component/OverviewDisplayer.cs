using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Graph;
using LocalUtilities.TypeToolKit.Mathematic;
using System.Drawing.Drawing2D;
using WarringStates.Events;
using WarringStates.Graph;
using WarringStates.Map;
using OpenCvSharp.Extensions;
using System.Diagnostics;

namespace WarringStates.UI.Component;

public partial class OverviewDisplayer : Displayer
{
    bool FullScreen { get; set; } = true;

    Bitmap? OverviewCache { get; set; }

    Rectangle DisplayRect { get; set; }

    GridUpdatedArgs? GridUpdatedArgs { get; set; }

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
        LocalEvents.Hub.AddListener<GridUpdatedArgs>(LocalEvents.Graph.GridUpdated, Relocate);
    }

    private void SetBounds(GameDisplayerUpdatedArgs args)
    {
        DisplayRect = args.DisplayRect;
        if (FullScreen)
        {
            Size = Atlas.Size.ScaleSizeOnRatio(DisplayRect.Size);
            Location = new(DisplayRect.Left + (DisplayRect.Width - Width) / 2, DisplayRect.Top + (DisplayRect.Height - Height) / 2);
        }
        else
        {
            var size = new Size((int)(DisplayRect.Width * 0.25), (int)(DisplayRect.Height * 0.25));
            Size = Atlas.Size.ScaleSizeOnRatio(size);
            Location = new(DisplayRect.Right - Width, DisplayRect.Top);
        }
    }

    private void Relocate(GridUpdatedArgs args)
    {
        if (Width is 0 || Height is 0)
            return;
        GridUpdatedArgs = args;
        Relocate();
        RelocateOverview();
        RelocateFocus(args.DrawRect, args.Origin);
        Invalidate();
    }

    private void RelocateOverview()
    {
        var stop = new Stopwatch();
        if (OverviewCache is not null && Size == OverviewCache.Size)
        {
            stop.Restart();
            OverviewCache.TemplateDrawPartsOnSharp((Bitmap)Image, LastFocusOnRects, true);
            stop.Stop();
            LocalEvents.Hub.Broadcast(LocalEvents.Test.AddSingleInfo, new TestForm.TestInfo("point bitmap redraw", stop.ElapsedMilliseconds.ToString()));
            stop.Start();
            //Image = OverviewCache.TemplateDrawPartsOn((Bitmap)Image, LastFocusOnRects, true);
            stop.Stop();
            LocalEvents.Hub.Broadcast(LocalEvents.Test.AddSingleInfo, new TestForm.TestInfo("cv2 redraw", stop.ElapsedMilliseconds.ToString()));
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
        stop.Start();
        OverviewCache = OverviewCache.CopyToNewSize(Width, Height);
        stop.Stop();
        LocalEvents.Hub.Broadcast(LocalEvents.Test.AddSingleInfo, new TestForm.TestInfo("cv2", stop.ElapsedMilliseconds.ToString()));
        stop.Restart();
        OverviewCache = OverviewCache.CopyToNewSizeSharp(Width, Height, InterpolationMode.Low);
        stop.Stop();
        LocalEvents.Hub.Broadcast(LocalEvents.Test.AddSingleInfo, new TestForm.TestInfo("graphics", stop.ElapsedMilliseconds.ToString()));

        stop.Restart();
        Image = OverviewCache.Clone() as Bitmap;
        stop.Stop();
        LocalEvents.Hub.Broadcast(LocalEvents.Test.AddSingleInfo, new TestForm.TestInfo("clone", stop.ElapsedMilliseconds.ToString()));
        stop.Restart();
        //Image = OverviewCache.Copy();
        stop.Stop();
        LocalEvents.Hub.Broadcast(LocalEvents.Test.AddSingleInfo, new TestForm.TestInfo("cv2 copy", stop.ElapsedMilliseconds.ToString()));

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
        var edgeLength = (double)LatticeGrid.CellEdgeLength;
        var width = drawRect.Width / edgeLength;
        var height = drawRect.Height / edgeLength;
        var x = Atlas.Width - origin.X / edgeLength;
        var y = Atlas.Height - origin.Y / edgeLength;
        var widthRatio = Atlas.Width / (double)Width;
        var heightRatio = Atlas.Height / (double)Height;
        FocusScaleRatio = (widthRatio * edgeLength, heightRatio * edgeLength);
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
