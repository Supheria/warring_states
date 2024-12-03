using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace WarringStates.Server.GUI.Views;

public partial class SwitchServerButton : Button
{
    protected override Type StyleKeyOverride => typeof(Button);

    public SwitchServerButton()
    {
        InitializeComponent();
    }
}