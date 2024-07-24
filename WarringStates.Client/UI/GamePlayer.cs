using AltitudeMapGenerator;
using LocalUtilities.SimpleScript;
using LocalUtilities.SimpleScript.Common;
using LocalUtilities.TypeGeneral;
using WarringStates.Client.Events;
using WarringStates.Client.Graph;
using WarringStates.Client.Map;
using WarringStates.Client.UI.Component;

namespace WarringStates.Client.UI;

public partial class GamePlayer : Pannel
{
    Settings Settings { get; } = new();

    ToolBar ToolBar { get; } = new();

    GamePlane GamePlane { get; } = new();

    Overview Overview { get; } = new();

    InfoBar InfoBar { get; } = new();

    public int ToolBarHeight { get; set; } = 30;

    public int InfoBarHeight { get; set; } = 100;

    //GridDrawer Grid { get; } = new();

    public GamePlayer()
    {
        KeyDown += KeyPressed;
        Controls.AddRange([
            Settings,
            ToolBar,
            Overview,
            GamePlane,
            InfoBar,
        ]);
        Fortest();
    }

    private void Fortest()
    {
        var map = SerializeTool.DeserializeFile<AltitudeMap>(new(nameof(AltitudeMap)), "altitude map", new SsSignTable());
        Atlas.Relocate(map, new(1000));
    }

    public void EnableListener()
    {
        Settings.EnableListener();
        ToolBar.EnableListener();
        GamePlane.EnableListener();
        Overview.EnableListener();
        InfoBar.EnableListener();
    }

    public void DisableListener()
    {
        Settings.DisableListener();
        ToolBar.DisableListener();
        GamePlane.DisableListener();
        Overview.DisableListener();
        InfoBar.DisableListener();
    }

    private void KeyPressed(object? sender, KeyEventArgs e)
    {

    }

    protected override void SetSize()
    {
        //
        ToolBar.Bounds = new(
            ClientLeft,
            ClientTop,
            ClientWidth,
            ToolBarHeight
            );
        //
        GamePlane.Bounds = new(
            ClientLeft,
            ToolBar.Bottom,
            ClientWidth,
            Height - ToolBar.Height - InfoBar.Height
            );
        //
        InfoBar.Bounds = new(
            ClientLeft,
            ClientHeight - InfoBar.Height,
            ClientWidth,
            InfoBarHeight
            );
        //
        Overview.Bounds = new(
            ClientLeft, 
            ToolBar.Bottom,
            ClientWidth,
            ClientHeight - ToolBar.Height);
    }
}
