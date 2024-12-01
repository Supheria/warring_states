using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace WarringStates.Server.GUI.ViewModels;

public partial class ThumbnailViewModel : ViewModelBase
{
    [ObservableProperty]
    int _selectedIndex;

    [ObservableProperty]
    int _width;

    [ObservableProperty]
    int _height;

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        switch (e.PropertyName)
        {

        }
    }
}
