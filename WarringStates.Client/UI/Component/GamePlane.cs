using LocalUtilities.TypeGeneral;
using WarringStates.Client.Events;
using WarringStates.Client.Graph;

namespace WarringStates.Client.UI.Component;

public partial class GamePlane : Displayer
{
    //Point? MouseLocation { get; set; } = null;

    public override void Redraw()
    {
        base.Redraw();
        BeginDrawGrid();
    }

    private void BeginDrawGrid()
    {
        GridDrawer.Redraw(ClientSize, BackColor);
    }

    private void EndDrawGrid(GridRedrawArgs args)
    {
        Invoke(() =>
        {
            Image?.Dispose();
            Image = (Image)args.Source.Clone();
            Update();
        });
    }
}
