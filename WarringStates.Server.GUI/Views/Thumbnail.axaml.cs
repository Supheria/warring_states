using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using WarringStates.Server.GUI.ViewModels;

namespace WarringStates.Server.GUI.Views;

public partial class Thumbnail : Control
{
    public ThumbnailViewModel? ViewModel => DataContext as ThumbnailViewModel;

    public Thumbnail()
    {
        InitializeComponent();
    }

    public override void Render(DrawingContext drawer)
    {
        base.Render(drawer);
        //Width = Bounds.Width;
        //Height = Bounds.Height;
        drawer.FillRectangle(new SolidColorBrush(Color.FromRgb(100, 100, 100)), new(0, 0, Bounds.Width, Bounds.Height));
    }
}