using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using WarringStates.Events;
using WarringStates.Flow;
using WarringStates.Graph;
using WarringStates.UI.Component;

namespace WarringStates.UI;

public partial class MainForm : ResizeableForm
{
    public enum States
    {
        SelectArchive,
        PlayGame
    }

    States CurrentSatate { get; set; } = States.SelectArchive;

    public override string LocalName => nameof(MainForm);

    public override Size MinimumSize { get; set; } = new(200, 200);

    ArchiveDisplayer ArchiveSelector { get; } = new();

    ToolBrandDisplayer ToolBrand { get; } = new();

    GameDisplayer GamePlane { get; } = new();

    OverviewDisplayer Overview { get; } = new();

    InfoBrandDisplayer InfoBrand { get; } = new();

    LatticeGrid Grid { get; } = new(new(), new());

    SpanFlow SpanFlow { get; set; } = new();

    protected override void InitializeComponent()
    {
        MinimumSize = new(860, 530);
        OnDrawingClient += DrawClient;
        OnLoadForm += LoadForm;
        OnSaveForm += SaveForm;
        Shown += GameForm_Shown;
        Controls.Add(ArchiveSelector);
        ArchiveSelector.EnableListener();
        LocalEvents.Hub.AddListener<States>(LocalEvents.UserInterface.MainFormSwitchState, SwitchState);
    }

    private void SwitchState(States state)
    {
        if (state == CurrentSatate)
            return;
        SuspendLayout();
        Controls.Clear();
        CurrentSatate = state;
        switch (CurrentSatate)
        {
            case States.SelectArchive:
                Controls.Add(ArchiveSelector);
                break;
            case States.PlayGame:
                Controls.AddRange([
                    ToolBrand,
                    GamePlane,
                    Overview,
                    InfoBrand,
                ]);
                Grid.EnableListner();
                ToolBrand.EnableListener();
                Overview.EnableListener();
                GamePlane.EnableListener();
                break;
            default:
                break;
        }
        DrawClient();
        ResumeLayout();
    }

    private void SaveForm(SsSerializer serializer)
    {
        serializer.WriteTag(nameof(SpanFlow.CurrentSpan), SpanFlow.CurrentSpan.ToString());
        //serializer.WriteObject(Grid);
    }

    private void LoadForm(SsDeserializer deserializer)
    {
        var startSpan = deserializer.ReadTag(nameof(SpanFlow.CurrentSpan), int.Parse);
        //SpanFlow.Relocate(startSpan);
        //deserializer.ReadObject(Grid);
    }

    private void GameForm_Shown(object? sender, EventArgs e)
    {
        LocalEvents.Hub.Broadcast(LocalEvents.Flow.SwichFlowState);
    }

    private void DrawClient()
    {
        if (Math.Min(ClientSize.Width, ClientSize.Height) is 0)
            return;
        LocalEvents.Hub.Broadcast(LocalEvents.UserInterface.MainFormOnResize, ClientRectangle);
    }
}
