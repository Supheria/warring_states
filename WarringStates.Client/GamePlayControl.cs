using WarringStates.Client.Component;
using WarringStates.Client.Graph;
using WarringStates.Client.Map;

namespace WarringStates.Client;

public partial class GamePlayControl : UserControl
{
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
        //LocalEvents.Hub.TryAddListener(LocalEvents.UserInterface.StartGamePlay, StartGame);
        //LocalEvents.Hub.TryAddListener(LocalEvents.UserInterface.FinishGamePlay, FinishGame);
        //ArchiveManager.ReLocate();
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
        LocalEvents.Hub.TryBroadcast(LocalEvents.UserInterface.MainFormOnDraw, ClientRectangle);
    }
}
