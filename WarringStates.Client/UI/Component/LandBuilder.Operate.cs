using LocalUtilities.TypeGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Client.Events;
using WarringStates.Client.Net;
using WarringStates.Data;
using WarringStates.Map;

namespace WarringStates.Client.UI.Component;

internal partial class LandBuilder
{
    SourceLandCanBuildArgs BuildArgs { get; set; } = new();

    public override void EnableListener()
    {
        base.EnableListener();
        LocalEvents.TryAddListener<SourceLandCanBuildArgs>(LocalEvents.UserInterface.SourceLandCanBuild, ShowCanBuildTypes);
    }

    public override void DisableListener()
    {
        base.DisableListener();
        LocalEvents.TryRemoveListener<SourceLandCanBuildArgs>(LocalEvents.UserInterface.SourceLandCanBuild, ShowCanBuildTypes);
    }

    protected override void AddOperation()
    {
        base.AddOperation();
        CloseButton.Click += CloseButton_Click;
        BuildButton.Click += BuildButton_Click;
        BuildTypeSelector.IndexChanged += BuildTypeSelector_IndexChanged;
    }

    private void BuildTypeSelector_IndexChanged(object? sender, EventArgs e)
    {
        if (BuildTypeSelector.SelectedIndex is -1)
            BuildButton.CanSelect = false;
        else
            BuildButton.CanSelect = true;
    }

    private void BuildButton_Click(object? sender, EventArgs e)
    {
        LocalNet.Service.BuildLand(BuildArgs.Site, BuildArgs.CanbuildTypes[BuildTypeSelector.SelectedIndex]);
    }

    private void ShowCanBuildTypes(SourceLandCanBuildArgs args)
    {
        BeginInvoke(() =>
        {
            Visible = true;
            Bounds = Range;
            BuildArgs = args;
            Relocate();
        });
    }

    private void CloseButton_Click(object? sender, EventArgs e)
    {
        Visible = false;
        Bounds = Range;
    }

    public void Relocate()
    {
        if (BuildArgs.CanbuildTypes.Length is 0)
            BuildButton.Text = Localize.Table.NoBuildableType;
        else
            BuildButton.Text = Localize.Table.BuildLand;
        BuildButton.Redraw();
        BuildButton.Invalidate();
        BuildTypeSelector.ItemList = BuildArgs.CanbuildTypes.Select(x => Localize.Table.ConvertEnum(x)).ToList();
        BuildTypeSelector.SelectedIndex = -1;
        BuildTypeSelector.Redraw();
        BuildTypeSelector.Invalidate();
    }
}
