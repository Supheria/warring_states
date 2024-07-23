using AltitudeMapGenerator;
using LocalUtilities.SimpleScript;
using LocalUtilities.SimpleScript.Common;
using WarringStates.Client.Events;
using WarringStates.Client.Graph;
using WarringStates.Client.Map;
using WarringStates.Client.UI.Component;

namespace WarringStates.Client.UI;

public partial class GamePlayer : Control
{
    Settings Settings { get; } = new();

    ToolBar ToolBar { get; } = new();

    GamePlane GamePlane { get; } = new();

    Overview Overview { get; } = new();

    InfoBar InfoBar { get; } = new();

    GridDrawer Grid { get; } = new();

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
        Controls.Clear();
        Controls.AddRange([
            Settings,
            ToolBar,
            Overview,
            GamePlane,
            InfoBar,
        ]);
        //Redraw();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        BeginInvoke(SetSize);
    }

    private void SetSize()
    {
        if (Math.Min(ClientSize.Width, ClientSize.Height) is 0)
            return;
        //
        ToolBar.Bounds = new(
            Left,
            Top,
            Width,
            ToolBar.Height
            );
        //
        GamePlane.Bounds = new(
            Left,
            ToolBar.Bottom,
            Width,
            Height - ToolBar.Height - InfoBar.Height
            );
        //
        InfoBar.Bounds = new(
            Left,
            Height - InfoBar.Height,
            Width,
            InfoBar.Height
            );
        //
        var bounds = new Rectangle(Left, ToolBar.Bottom, Width, Height);
        Overview.SetBounds(bounds);
    }
}
