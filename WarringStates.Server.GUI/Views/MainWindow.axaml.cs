using Avalonia.Controls;
using LocalUtilities.GUICore;

namespace WarringStates.Server.GUI.Views;

public partial class MainWindow : InitializeableWindow
{
    public override string InitializeName => nameof(MainWindow);

    public MainWindow()
    {
        InitializeComponent();
    }

    public Grid Grid { get; set; } = new();
}