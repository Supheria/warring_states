using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WarringStates.Events;
using WarringStates.Graph;
using WarringStates.UI.Component;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
using WarringStates.User;
using WarringStates.Map;

namespace WarringStates_Client;

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
        LocalSaves.ReLocate();
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
            //Overview.EnableListener();
            InfoBar.EnableListener();
            DrawClient();
        });
    }

    private void FinishGame()
    {
        Controls.Clear();
        Settings.DisableListener();
        ToolBar.DisableListener();
        GamePlane.DisableListener();
        Overview.DisableListener();
        InfoBar.DisableListener();
        DrawClient();
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
