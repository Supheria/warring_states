using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using LocalUtilities.SimpleScript;
using System;
using System.Collections.Generic;
using System.IO;
using WarringStates.Map;
using WarringStates.Server.GUI.Models;

namespace WarringStates.Server.GUI.Views;

public partial class ArchiveList : ListBox
{
    protected override Type StyleKeyOverride => typeof(ListBox);

    public ArchiveList()
    {
        InitializeComponent();
    }
}
