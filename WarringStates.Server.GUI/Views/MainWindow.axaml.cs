using Avalonia;
using Avalonia.Controls;
using LocalUtilities.GUICore;
using System;
using System.Windows.Input;
using WarringStates.Server.GUI.Models;

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
