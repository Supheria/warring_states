using LocalUtilities.TypeGeneral;
using WarringStates.Client.Events;
using WarringStates.UI;

namespace WarringStates.Client.UI.Component;

public class Settings : Displayer
{
    public bool DoSetting { get; set; } = false;

    Rectangle Range { get; set; } = new();

    public void EnableListener()
    {
        //LocalEvents.TryAddListener<Rectangle>(LocalEvents.UserInterface.GamePlayControlOnDraw, SetBounds);
        LocalEvents.TryAddListener<KeyPressArgs>(LocalEvents.UserInterface.KeyPressed, KeyPress);
    }

    public void DisableListener()
    {
        //LocalEvents.TryRemoveListener<Rectangle>(LocalEvents.UserInterface.GamePlayControlOnDraw, SetBounds);
        LocalEvents.TryRemoveListener<KeyPressArgs>(LocalEvents.UserInterface.KeyPressed, KeyPress);
    }

    //private void SetBounds(Rectangle rect)
    //{
    //    if (DoSetting)
    //    {

    //    }
    //    else
    //        Bounds = new(0, 0, 0, 0);
    //    Range = rect;
    //    LocalEvents.TryBroadcast(LocalEvents.UserInterface.SettingsOnSetBounds, rect);
    //}

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
