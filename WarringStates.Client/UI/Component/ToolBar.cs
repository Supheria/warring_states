using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using WarringStates.Client.Events;
using WarringStates.Flow;

namespace WarringStates.Client.UI.Component;

public class ToolBar : Displayer
{
    bool DoSetting { get; set; } = false;

    Rectangle Range { get; set; } = new();

    Date CurrentDate { get; set; } = new();

    SolidBrush DateBrush { get; } = new(Color.White);

    public ToolBar()
    {
        //Height = 30;
    }

    public void EnableListener()
    {
        //LocalEvents.TryAddListener<Rectangle>(LocalEvents.UserInterface.GamePlayControlOnDraw, SetBounds);
        LocalEvents.TryAddListener<SpanFlowTickOnArgs>(LocalEvents.Flow.SpanFlowTickOn, SetDate);
    }

    public void DisableListener()
    {
        //LocalEvents.TryRemoveListener<Rectangle>(LocalEvents.UserInterface.GamePlayControlOnDraw, SetBounds);
        LocalEvents.TryRemoveListener<SpanFlowTickOnArgs>(LocalEvents.Flow.SpanFlowTickOn, SetDate);
    }

    private void SetBounds(Rectangle rect)
    {
        Range = rect;
        Width = rect.Width;
        //Relocate();
        Redraw();
        rect = new Rectangle(rect.Left, rect.Top + Height, rect.Width, rect.Height - Height);
        //LocalEvents.TryBroadcast(LocalEvents.UserInterface.ToolBarOnSetBounds, rect);
    }

    private void SetDate(SpanFlowTickOnArgs args)
    {
        CurrentDate = args.CurrentDate;
        Redraw();
    }

    public override void Redraw()
    {
        base.Redraw();
        var dateWidth = (ClientWidth * 0.2).ToRoundInt();
        var dateRect = new Rectangle(ClientLeft + ClientWidth - dateWidth, ClientTop, dateWidth, ClientHeight);
        using var g = Graphics.FromImage(Image);
        g.FillRectangle(new SolidBrush(Color.LightSlateGray), dateRect);
        var format = new StringFormat()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
        };
        g.DrawString(CurrentDate.ToString(), LabelFontData, DateBrush, dateRect, format);
    }
}
