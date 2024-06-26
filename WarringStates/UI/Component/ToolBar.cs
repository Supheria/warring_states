﻿using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using WarringStates.Events;
using WarringStates.Flow;

namespace WarringStates.UI.Component;

public class ToolBar : Displayer
{
    bool DoSetting { get; set; } = false;

    Rectangle Range { get; set; } = new();

    Date CurrentDate { get; set; } = new();

    SolidBrush DateBrush { get; } = new(Color.White);

    public ToolBar()
    {
        Height = 30;
        LocalEvents.Hub.AddListener<Rectangle>(LocalEvents.UserInterface.SettingsOnSetBounds, SetBounds);
        LocalEvents.Hub.AddListener<SpanFlowTickOnArgs>(LocalEvents.Flow.SpanFlowTickOn, SetDate);
    }

    private void SetBounds(Rectangle rect)
    {
        Range = rect;
        Width = rect.Width;
        Relocate();
        DrawDate();
        rect = new Rectangle(rect.Left, rect.Top + Height, rect.Width, rect.Height - Height);
        LocalEvents.Hub.Broadcast(LocalEvents.UserInterface.ToolBarOnSetBounds, rect);
    }

    private void SetDate(SpanFlowTickOnArgs args)
    {
        CurrentDate = args.CurrentDate;
        DrawDate();
    }

    private void DrawDate()
    {
        var dateWidth = (Width * 0.2).ToRoundInt();
        var dateRect = new Rectangle(Right - dateWidth, Top, dateWidth, Height);
        using var g = Graphics.FromImage(Image);
        g.FillRectangle(new SolidBrush(Color.LightSlateGray), dateRect);
        var format = new StringFormat()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
        };
        g.DrawString(CurrentDate.ToString(), LabelFontData, DateBrush, dateRect, format);
        Invalidate();
    }
}
