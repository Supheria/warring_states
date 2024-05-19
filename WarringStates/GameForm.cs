using LocalUtilities.TypeGeneral;

namespace WarringStates;

internal class GameForm : ResizeableForm
{
    public override string LocalName { get; set; } = nameof(GameForm);

    public override Size MinimumSize { get; set; } = new(200, 200);

    GameDisplayer Displayer { get; } = new();

    protected override void InitializeComponent()
    {
        OnDrawingClient += DrawClient;
        Controls.Add(Displayer);
    }

    private void DrawClient()
    {
        if (Math.Min(ClientRectangle.Width, ClientRectangle.Height) <= 0)
            return;
        Displayer.Bounds = new(Left, Top, Width, Height);
    }
}
