﻿using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Graph;

namespace WarringStates.UI;

public class ImageButton : Displayer
{
    public new string Text { get; set; } = "Null";

    new bool Focused { get; set; } = false;

    public new bool CanSelect { get; set; } = false;

    protected override FontData LabelFontData { get; set; } = new()
    {
        Size = 25f,
        Style = FontStyle.Bold,
    };

    public override void Redraw()
    {
        base.Redraw();
        using var g = Graphics.FromImage(Image);
        var rect = new Rectangle(0, 0, Width, Height);
        if (CanSelect && Focused)
        {
            g.Clear(BackColor);
            g.DrawUniformString(rect, Text, Height * 0.618f, FrontColor, LabelFontData);
        }
        else
        {
            g.Clear(FrontColor);
            g.DrawUniformString(rect, Text, Height * 0.618f, BackColor, LabelFontData);
        }
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        Focused = true;
        Redraw();
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        Focused = false;
        Redraw();
        Invalidate();
    }

    protected override void OnClick(EventArgs e)
    {
        if (CanSelect)
            base.OnClick(e);
    }
}
