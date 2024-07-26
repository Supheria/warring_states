using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using WarringStates.Client.Events;
using WarringStates.Client.Map;
using WarringStates.UI;

namespace WarringStates.Client.UI.Component;

public class Settings : Displayer
{
    public bool DoSetting
    {
        get => Visible;
        set => Visible = value;
    }

    Rectangle Range { get; set; } = new();

    public new Rectangle Bounds
    {
        get => base.Bounds;
        set
        {
            Range = value;
            if (DoSetting)
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
        }
    }

    public Settings()
    {
        DoSetting = false;
    }

    public override void EnableListener()
    {
        base.EnableListener();
        LocalEvents.TryAddListener<KeyPressArgs>(LocalEvents.UserInterface.KeyPressed, KeyPress);
    }

    public override void DisableListener()
    {
        base.DisableListener();
        LocalEvents.TryRemoveListener<KeyPressArgs>(LocalEvents.UserInterface.KeyPressed, KeyPress);
    }

    private new void KeyPress(KeyPressArgs args)
    {
        if (args.Value is not Keys.Escape)
            return;
        if (DoSetting)
        {
            Bounds = new(0, 0, 0, 0);
        }
        else
        {
            Bounds = Range;
        }
        DoSetting = !DoSetting;
    }

    public override void Redraw()
    {
        base.Redraw();

    }
}
