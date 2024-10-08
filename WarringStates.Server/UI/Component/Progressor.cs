﻿using AltitudeMapGenerator;
using LocalUtilities.TypeGeneral;

namespace WarringStates.Server.UI.Component;

internal class Progressor : Displayer, IProgressor
{
    public override Color BackColor { get; set; } = Color.White;

    public override Color FrontColor { get; set; } = Color.Blue;

    public Color BordColor { get; set; } = Color.Black;

    float Totol { get; set; } = 0f;

    int Now { get; set; } = 0;

    public Progressor()
    {
        Visible = false;
    }

    public void Reset(int total)
    {
        Totol = total;
        Now = 0;
    }

    public void Progress(int addon)
    {
        BeginInvoke(() =>
        {
            Now += addon;
            DrawProgress(Now / Totol);
            Invalidate();
        });
    }

    public override void Redraw()
    {
        base.Redraw();
        DrawProgress(Now / Totol);
    }

    private void DrawProgress(float percent)
    {
        if (!Visible)
            return;
        var rect = new RectangleF(0, 0, Width * percent, Height);
        using var g = Graphics.FromImage(Image);
        g.Clear(BackColor);
        g.FillRectangle(new SolidBrush(FrontColor), rect);
        g.DrawRectangle(new Pen(BordColor, 3f), Bounds);
        //g.DrawUniformString(new(0, 0, Width, Height), Math.Round( percent, 2).ToString(), 0.33f * Width, FrontColor, new FontData());
    }
}
