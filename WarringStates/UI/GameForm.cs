using LocalUtilities.TypeGeneral;
using WarringStates.Events;
using WarringStates.Graph;

namespace WarringStates.UI;

public class GameForm : ResizeableForm
{
    public override string LocalName => nameof(GameForm);

    public override Size MinimumSize { get; set; } = new(200, 200);

    GameDisplayer Displayer { get; } = new();

    OverviewDisplayer Overview { get; } = new();

    LatticeGrid Grid { get; } = new();

    protected override void InitializeComponent()
    {
        OnDrawingClient += DrawClient;
        Controls.AddRange([
            Overview,
            Displayer,
            ]);
        Controls.SetChildIndex(Overview, 0);
        Controls.SetChildIndex(Displayer, 1);
        Grid.EnableListner();
        Overview.EnableListener();
        Displayer.EnableListener();
    }

    private void DrawClient()
    {
        if (Math.Min(ClientSize.Width, ClientSize.Height) is 0)
            return;
        LocalEvents.Global.Broadcast(LocalEventTypes.Global.GameFormUpdate, new GameFormUpdateCallback(ClientSize));

        //LocalEvents.ForTest();
    }
}
