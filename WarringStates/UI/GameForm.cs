using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using WarringStates.Events;
using WarringStates.Flow;
using WarringStates.Graph;
using WarringStates.UI.Component;

namespace WarringStates.UI;

public partial class GameForm : ResizeableForm
{
    public override string LocalName => nameof(GameForm);

    public override Size MinimumSize { get; set; } = new(200, 200);

    ToolBrandDisplayer ToolBrand { get; } = new();

    GameDisplayer GamePlane { get; } = new();

    OverviewDisplayer Overview { get; } = new();

    InfoBrandDisplayer InfoBrand { get; } = new();

    LatticeGrid Grid { get; } = new();

    SpanFlow Tick { get; set; } = new();

    protected override void InitializeComponent()
    {
        OnDrawingClient += DrawClient;
        OnLoadForm += LoadForm;
        OnSaveForm += SaveForm;
        Controls.AddRange([
            ToolBrand,
            GamePlane,
            Overview,
            InfoBrand,
            ]);
        Controls.SetChildIndex(Overview, 0);
        Controls.SetChildIndex(GamePlane, 1);
        Grid.EnableListner();
        Overview.EnableListener();
        GamePlane.EnableListener();
        Shown += GameForm_Shown;
    }

    private void SaveForm(SsSerializer serializer)
    {
        serializer.WriteTag(nameof(Tick.CurrentSpan), Tick.CurrentSpan.ToString());
    }

    private void LoadForm(SsDeserializer deserializer)
    {
        var startSpan = deserializer.ReadTag(nameof(Tick.CurrentSpan), int.Parse);
        Tick.Relocate(startSpan);
    }

    private void GameForm_Shown(object? sender, EventArgs e)
    {
        LocalEvents.Hub.Broadcast(LocalEvents.Flow.SwichFlowState);
    }

    private void DrawClient()
    {
        if (Math.Min(ClientSize.Width, ClientSize.Height) is 0)
            return;
        ToolBrand.SetSize(new(ClientSize.Width, 50));
        var gameRect = new Rectangle(0, ClientRectangle.Top + ToolBrand.Height, ClientSize.Width, ClientSize.Height - ToolBrand.Height);
        LocalEvents.Hub.Broadcast(LocalEvents.UserInterface.GameFormUpdate, new GameFormUpdateArgs(gameRect));

        //LocalEvents.ForTest();
    }
}
