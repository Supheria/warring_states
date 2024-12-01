using LocalUtilities.General;
using WarringStates.Client.Events;
using WarringStates.Client.Map;

namespace WarringStates.Client.UI.Component;

public class Settings : Displayer
{
    Rectangle Range { get; set; } = new();

    public new Rectangle Bounds
    {
        get => base.Bounds;
        set
        {
            Range = value;
            if (Visible)
            {
                var size = GeometryTool.ScaleSizeWithinRatio(AtlasEx.Size, Range.Size);
                var location = new Point((int)(Range.Left + (Range.Width - size.Width) * 0.5f), (int)(Range.Top + (Range.Height - size.Height) * 0.5f));
                base.Bounds = new(location, size);
            }
            else
            {
                base.Bounds = new();
            }
        }
    }

    public Settings()
    {
        Visible = false;
    }

    public override void EnableListener()
    {
        base.EnableListener();
        LocalEvents.TryAddListener<KeyEventArgs>(LocalEvents.UserInterface.KeyPressed, KeyPress);
    }

    public override void DisableListener()
    {
        base.DisableListener();
        LocalEvents.TryRemoveListener<KeyEventArgs>(LocalEvents.UserInterface.KeyPressed, KeyPress);
    }

    private new void KeyPress(KeyEventArgs args)
    {
        if (args.KeyData is not Keys.Escape)
            return;
        Visible = !Visible;
        Bounds = Range;
    }

    public override void Redraw()
    {
        if (!Visible)
            return;
        base.Redraw();

    }
}
