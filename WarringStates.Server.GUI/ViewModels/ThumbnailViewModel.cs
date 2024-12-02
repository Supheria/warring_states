using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using LocalUtilities.GUICore;
using System;

namespace WarringStates.Server.GUI.ViewModels;

internal partial class ThumbnailViewModel : ViewModelBase
{
    [ObservableProperty]
    Color _BackColor = Colors.DimGray;

    [ObservableProperty]
    Color _FrontColor = Colors.White;

    [ObservableProperty]
    Bitmap? _Source = null;

    [ObservableProperty]
    Rect _Bounds = new();

    [ObservableProperty]
    double _BorderThickness = 10;

    public void SetSourceLoading()
    {
        var width = (int)Bounds.Width;
        var height = (int)Bounds.Height;
        if (width is 0 || height is 0)
            return;
        var loading = new WriteableBitmap(new(width, height), new(96, 96));
        using var lockedBuffer = loading.Lock();
        var random = new Random();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var color = random.Next() < random.Next() ? BackColor : FrontColor;
                lockedBuffer.SetPixel(i, j, new(color.A, color.R, color.G, color.B));
            }
        }
        Source = loading;
    }
}
