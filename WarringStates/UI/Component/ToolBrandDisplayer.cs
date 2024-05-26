using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using WarringStates.Events;
using WarringStates.Flow;

namespace WarringStates.UI.Component;

public class ToolBrandDisplayer : Displayer
{
    Date CurrentDate { get; set; }

    SolidBrush DateBrush { get; } = new(Color.White);

    public ToolBrandDisplayer()
    {
        LocalEvents.Hub.AddListener<GameFormUpdateArgs>(LocalEvents.UserInterface.GameFormUpdate, SetWidth);
        LocalEvents.Hub.AddListener<SpanFlowTickOnArgs>(LocalEvents.Flow.SpanFlowTickOn, SetDate);
        Height = 30;
    }

    private void SetWidth(GameFormUpdateArgs args)
    {
        Width = args.GameRect.Width;
        Relocate();
        DrawDate();
    }

    private void SetDate(SpanFlowTickOnArgs args)
    {
        CurrentDate = args.CurrentDate;
        DrawDate();
    }

    private void DrawDate()
    {
        var dateWidth = (Width * 0.2).ToInt();
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
