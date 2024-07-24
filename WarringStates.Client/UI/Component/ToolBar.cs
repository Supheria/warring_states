using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using WarringStates.Client.Events;
using WarringStates.Flow;

namespace WarringStates.Client.UI.Component;

public class ToolBar : Displayer
{
    Date CurrentDate { get; set; } = new();

    SolidBrush DateBrush { get; } = new(Color.White);

    public override void EnableListener()
    {
        base.EnableListener();
        LocalEvents.TryAddListener<SpanFlowTickOnArgs>(LocalEvents.Flow.SpanFlowTickOn, SetDate);
    }

    public override void DisableListener()
    {
        base.DisableListener();
        LocalEvents.TryRemoveListener<SpanFlowTickOnArgs>(LocalEvents.Flow.SpanFlowTickOn, SetDate);
    }

    private void SetDate(SpanFlowTickOnArgs args)
    {
        CurrentDate = args.CurrentDate;
        Redraw();
        Invalidate();
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
