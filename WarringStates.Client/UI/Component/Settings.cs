using LocalUtilities.TypeGeneral;
using WarringStates.Client.Events;
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
}
