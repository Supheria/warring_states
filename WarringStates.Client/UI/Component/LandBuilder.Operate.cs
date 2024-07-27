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
    Coordinate PointOnCellSite { get; set; } = new();

    SourceLandTypes[] CanbuildTypes { get; set; } = [];

    public override void EnableListener()
    {
        base.EnableListener();
        LocalEvents.TryAddListener<GridCellPointedOnArgs>(LocalEvents.Graph.PointOnCell, PointOnCell);
        LocalEvents.TryAddListener<SourceLandTypes[]>(LocalEvents.UserInterface.SourceLandTypesCanBuild, ShowCanBuildTypes);
    }

    public override void DisableListener()
    {
        base.DisableListener();
        LocalEvents.TryRemoveListener<GridCellPointedOnArgs>(LocalEvents.Graph.PointOnCell, PointOnCell);
        LocalEvents.TryRemoveListener<SourceLandTypes[]>(LocalEvents.UserInterface.SourceLandTypesCanBuild, ShowCanBuildTypes);
    }

    protected override void AddOperation()
    {
        base.AddOperation();
        CloseButton.Click += CloseButton_Click;
        BuildButton.Click += BuildButton_Click;
    }

    private void BuildButton_Click(object? sender, EventArgs e)
    {
        LocalNet.Service.CheckBuildLand(PointOnCellSite);
    }

    private void ShowCanBuildTypes(SourceLandTypes[] types)
    {
        CanbuildTypes = types;
        BeginInvoke(Relocate);
    }

    private void CloseButton_Click(object? sender, EventArgs e)
    {
        Visible = false;
        Bounds = Range;
    }

    private void PointOnCell(GridCellPointedOnArgs args)
    {
        if (args.MouseOperate is MouseOperates.LeftDoubleClick)
        {
            Visible = true;
            Bounds = Range;
            PointOnCellSite = args.Site;
            LocalNet.Service.CheckBuildLand(PointOnCellSite);
        }
    }

    public void Relocate()
    {
        if (CanbuildTypes.Length is 0)
        {
            BuildButton.CanSelect = false;
            BuildButton.Text = Localize.Table.NoBuildableType;
        }
        else
        {
            BuildButton.CanSelect = true;
            BuildButton.Text = Localize.Table.BuildLand;
        }
        BuildButton.Redraw();
        BuildButton.Invalidate();
        BuildTypeSelector.ItemList = CanbuildTypes.Select(x => Localize.Table.ConvertEnum(x)).ToList();
        BuildTypeSelector.Redraw();
        BuildTypeSelector.Invalidate();
    }
}
