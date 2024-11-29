using Avalonia.Controls;

namespace WarringStates.Server.GUI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
    }

    public Grid Grid { get; set; } = new();
}