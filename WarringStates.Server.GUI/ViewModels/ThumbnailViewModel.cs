﻿using Avalonia.Media;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using LocalUtilities.GUICore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarringStates.Server.GUI.ViewModels;

internal partial class ThumbnailViewModel : ViewModelBase
{
    [ObservableProperty]
    Color _backColor = Colors.White;

    [ObservableProperty]
    Bitmap? _source = null;
}