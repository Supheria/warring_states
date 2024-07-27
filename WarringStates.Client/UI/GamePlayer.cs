using LocalUtilities.TypeGeneral;
using WarringStates.Client.Events;
using WarringStates.Client.UI.Component;

namespace WarringStates.Client.UI;

public partial class GamePlayer : Pannel
{
    Settings Settings { get; } = new();

    LandBuilder LandOperate { get; } = new();

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
            LandOperate,
            ToolBar,
            Overview,
            GamePlane,
            InfoBar,
        ]);
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
            ClientHeight - ToolBar.Height - InfoBar.Height
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
        //
        var width = ClientWidth / 3;
        LandOperate.Bounds = new(
            ClientRight - width,
            ClientTop,
            width,
            ClientHeight
            );
    }
}
