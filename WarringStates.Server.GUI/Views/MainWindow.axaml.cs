using LocalUtilities.GUICore;

namespace WarringStates.Server.GUI.Views;

public partial class MainWindow : InitializeableWindow
{
    public override string InitializeName => nameof(MainWindow);

    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        //AtlasEx.RefreshArchiveList();
    }
}
