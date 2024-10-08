﻿using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Graph;
using LocalUtilities.TypeToolKit.Mathematic;
using System.Drawing.Drawing2D;
using WarringStates.Client.Events;
using WarringStates.Client.Graph;
using WarringStates.Client.Map;

namespace WarringStates.Client.UI.Component;

public partial class Overview : Displayer
{
    bool FullScreen { get; set; } = false;

    Bitmap? OverviewCache { get; set; }

    Rectangle Range { get; set; }

    (double Width, double Height) FocusScaleRatio { get; set; }

    Rectangle FocusRect { get; set; }

    Color FocusColor { get; set; } = Color.Red;

    Rectangle GridDrawRect { get; set; } = new();

    Coordinate GridOrigin { get; set; } = new();

    public new Rectangle Bounds
    {
        get => base.Bounds;
        set
        {
            Visible = false;
            Range = value;
            if (FullScreen)
            {
                var size = GeometryTool.ScaleSizeWithinRatio(Atlas.Size, Range.Size);
                var location = new Point((int)(Range.Left + (Range.Width - size.Width) * 0.5f), (int)(Range.Top + (Range.Height - size.Height) * 0.5f));
                base.Bounds = new(location, size);
            }
            else
            {
                var size = new Size((int)(Range.Width * 0.25f), (int)(Range.Height * 0.25f));
                size = Atlas.Size.ScaleSizeWithinRatio(size);
                var location = new Point(Range.Right - size.Width, Range.Top);
                base.Bounds = new(location, size);
            }
            Visible = true;
        }
    }

    public override void Redraw()
    {
        Image?.Dispose();
        if (OverviewCache is not null && Size == OverviewCache.Size)
            Image = (Bitmap)OverviewCache.Clone();
        else
            RedrawOverview();
        RedrawFocus();
    }

    private void RedrawOverview()
    {
        OverviewCache?.Dispose();
        OverviewCache = Atlas.GetOverview(ClientSize);
        var temp = OverviewCache.CopyToNewSize(ClientSize, InterpolationMode.Low);
        OverviewCache.Dispose();
        OverviewCache = temp;
        Image = (Bitmap)OverviewCache.Clone();
    }

    private void RedrawFocus()
    {
        var edgeLength = (double)GridDrawer.CellEdgeLength;
        var width = GridDrawRect.Width / edgeLength;
        var height = GridDrawRect.Height / edgeLength;
        var x = Atlas.Width - GridOrigin.X / edgeLength;
        var y = Atlas.Height - GridOrigin.Y / edgeLength;
        var widthRatio = Atlas.Width / (double)ClientWidth;
        var heightRatio = Atlas.Height / (double)ClientHeight;
        FocusScaleRatio = (widthRatio * edgeLength, heightRatio * edgeLength);
        FocusRect = new Rectangle((x / widthRatio).ToRoundInt(), (y / heightRatio).ToRoundInt(), (width / widthRatio).ToRoundInt(), (height / heightRatio).ToRoundInt());
        using var g = Graphics.FromImage(Image);
        using var pen = new Pen(FocusColor, Math.Min(ClientWidth, ClientHeight) * 0.01f);
        foreach (var rect in GeometryTool.CutRectLoopRectsInRange(FocusRect, ClientRect))
        {
            g.DrawRectangle(pen, rect);
        }
    }
}
