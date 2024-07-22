using WarringStates.Client.Events;
using WarringStates.Client.Graph;
using WarringStates.Client.Map;
using WarringStates.Client.UI.Component;

namespace WarringStates.Client.UI;

public partial class GamePlayControl : UserControl
{
    ArchiveSelector ArchiveSelector { get; } = new();

    Settings Settings { get; } = new();

    ToolBar ToolBar { get; } = new();

    GamePlane GamePlane { get; } = new();

    Overview Overview { get; } = new();

    InfoBar InfoBar { get; } = new();

    LatticeGrid Grid { get; } = new();

    public GamePlayControl()
    {
        Resize += (_, _) => DrawClient();
        KeyDown += KeyPressed;
        //LocalEvents.TryAddListener(LocalEvents.UserInterface.StartGamePlay, StartGame);
        //LocalEvents.TryAddListener(LocalEvents.UserInterface.FinishGamePlay, FinishGame);
        //ArchiveManager.ReLocate();
        Controls.Add(ArchiveSelector);
        //ArchiveSelector.EnableListener();
    }

    public void StartGame()
    {
        var a = Atlas.Height;
        BeginInvoke(() =>
        {
            Controls.Clear();
            Controls.AddRange([
                Settings,
            ToolBar,
            Overview,
            GamePlane,
            InfoBar,
        ]);
            Settings.EnableListener();
            ToolBar.EnableListener();
            GamePlane.EnableListener();
            Overview.EnableListener();
            InfoBar.EnableListener();
            DrawClient();
        });
    }

    public void FinishGame()
    {
        BeginInvoke(() =>
        {
            Controls.Clear();
            Settings.DisableListener();
            ToolBar.DisableListener();
            GamePlane.DisableListener();
            Overview.DisableListener();
            InfoBar.DisableListener();
            DrawClient();
        });
    }

    private void KeyPressed(object? sender, KeyEventArgs e)
    {
        Controls.Clear();
        Controls.AddRange([
            Settings,
            ToolBar,
            Overview,
            GamePlane,
            InfoBar,
        ]);
        DrawClient();
    }

    private void DrawClient()
    {
        if (Math.Min(ClientSize.Width, ClientSize.Height) is 0)
            return;
        //LocalEvents.TryBroadcast(LocalEvents.UserInterface.GamePlayControlOnDraw, ClientRectangle);
    }
}
