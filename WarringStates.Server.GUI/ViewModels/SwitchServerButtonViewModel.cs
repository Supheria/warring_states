using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Server.GUI.Models;

namespace WarringStates.Server.GUI.ViewModels;

internal partial class SwitchServerButtonViewModel : ViewModelBase
{
    [ObservableProperty]
    string _Content = "开启";

    public SwitchServerButtonViewModel()
    {
        LocalNet.Server.OnStart += () => Content = "关闭";
        LocalNet.Server.OnClose += () => Content = "开启";
    }
}
