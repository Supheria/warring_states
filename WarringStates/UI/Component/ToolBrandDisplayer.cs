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
        Height = 30;
    }

    public void EnableListener()
    {
        LocalEvents.Hub.AddListener<Rectangle>(LocalEvents.UserInterface.MainFormOnResize, SetWidth);
        LocalEvents.Hub.AddListener<SpanFlowTickOnArgs>(LocalEvents.Flow.SpanFlowTickOn, SetDate);
    }

    private void SetWidth(Rectangle rect)
    {
        Width = rect.Width;
        Relocate();
        DrawDate();
        rect = new Rectangle(rect.Left, rect.Top + Height, rect.Width, rect.Height - Height);
        LocalEvents.Hub.Broadcast(LocalEvents.UserInterface.ToolBrandDisplayerOnResize, rect);
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
