using Avalonia.Controls;

namespace WarringStates.Server.GUI.Views;

internal partial class MainWindow : InitializeableWindow
{
    public override string InitializeName => nameof(MainWindow);

    public MainWindow()
    {
        InitializeComponent();
    }
}
