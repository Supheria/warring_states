using Avalonia.Media;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using LocalUtilities.GUICore;

namespace WarringStates.Server.GUI.ViewModels;

internal partial class ThumbnailViewModel : ViewModelBase
{
    [ObservableProperty]
    Color _backColor = Colors.White;

    [ObservableProperty]
    Bitmap? _source = null;
}
