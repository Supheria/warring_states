using Avalonia.Controls;
using System;

namespace WarringStates.Server.GUI.Views;

public partial class ArchiveList : ListBox
{
    protected override Type StyleKeyOverride => typeof(ListBox);

    public ArchiveList()
    {
        InitializeComponent();
    }
}
