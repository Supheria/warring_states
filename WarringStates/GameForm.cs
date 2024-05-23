using LocalUtilities.TypeGeneral;

namespace WarringStates;

public class GameForm : ResizeableForm
{
    public override string LocalName { get; set; } = nameof(GameForm);

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
    }

    private void DrawClient()
    {
        if (Math.Min(ClientSize.Width, ClientSize.Height) is 0)
            return;
        Overview.SetRange(ClientSize);
        Displayer.SetRange(ClientSize);
    }
}
