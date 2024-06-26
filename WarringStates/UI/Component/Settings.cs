﻿using LocalUtilities.TypeGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;
using WarringStates.Events;

namespace WarringStates.UI.Component;

public class Settings : Displayer
{
    public bool DoSetting { get; set; } = false;

    Rectangle Range { get; set; } = new();

    public void EnableListener()
    {
        LocalEvents.Hub.AddListener<Rectangle>(LocalEvents.UserInterface.MainFormOnDraw, SetBounds);
        LocalEvents.Hub.AddListener<Keys>(LocalEvents.UserInterface.KeyPressed, KeyPress);
    }

    public void DisableListener()
    {
        LocalEvents.Hub.TryRemoveListener<Rectangle>(LocalEvents.UserInterface.MainFormOnDraw, SetBounds);
        LocalEvents.Hub.TryRemoveListener<Keys>(LocalEvents.UserInterface.KeyPressed, KeyPress);
    }

    private void SetBounds(Rectangle rect)
    {
        if (DoSetting)
        {

        }
        else
            Bounds = new(0, 0, 0, 0);
        Range = rect;
        LocalEvents.Hub.Broadcast(LocalEvents.UserInterface.SettingsOnSetBounds, rect);
    }

    private new void KeyPress(Keys key)
    {
        if (key is not Keys.Escape)
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
