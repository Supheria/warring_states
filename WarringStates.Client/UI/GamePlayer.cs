using AltitudeMapGenerator;
using LocalUtilities.SimpleScript;
using LocalUtilities.SimpleScript.Common;
using LocalUtilities.TypeGeneral;
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

    public static int ToolBarHeight { get; set; } = 30;

    public static int InfoBarHeight { get; set; } = 100;

    public GamePlayer()
    {
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
        var map = SerializeTool.DeserializeFile<AltitudeMap>(new(nameof(AltitudeMap)), new SsSignTable(), "altitude map");
        Atlas.Relocate(map, new(1000));
    }

    public override void EnableListener()
    {
        base.EnableListener();
        Settings.EnableListener();
        ToolBar.EnableListener();
        GamePlane.EnableListener();
        Overview.EnableListener();
        InfoBar.EnableListener();
    }

    public override void DisableListener()
    {
        base.DisableListener();
        Settings.DisableListener();
        ToolBar.DisableListener();
        GamePlane.DisableListener();
        Overview.DisableListener();
        InfoBar.DisableListener();
    }

    protected override void SetSize()
    {
        base.SetSize();
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
        //
        Settings.Bounds = ClientRect;
    }
}
