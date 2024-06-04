using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using WarringStates.Events;
using WarringStates.Flow;
using WarringStates.Graph;
using WarringStates.Map;
using WarringStates.UI.Component;
using WarringStates.User;

namespace WarringStates.UI;

public partial class MainForm : ResizeableForm
{
    public override string LocalName => nameof(MainForm);

    public override Size MinimumSize { get; set; } = new(200, 200);

    ArchiveSelector ArchiveSelector { get; } = new();

    Settings Settings { get; } = new();

    ToolBar ToolBar { get; } = new();

    GamePlane GamePlane { get; } = new();

    Overview Overview { get; } = new();

    InfoBar InfoBar { get; } = new();

    LatticeGrid Grid { get; } = new();

    SpanFlow SpanFlow { get; set; } = new();

    AnimateFlow AnimateFlow { get; set; } = new();

    protected override void InitializeComponent()
    {
        MinimumSize = new(860, 530);
        OnDrawingClient += DrawClient;
        OnLoadForm += LoadForm;
        OnSaveForm += SaveForm;
        KeyDown += KeyPressed;
        Controls.Add(ArchiveSelector);
        ArchiveSelector.EnableListener();
        LocalEvents.Hub.AddListener<Archive>(LocalEvents.UserInterface.ArchiveSelected, RelodeArchive);
        LocalEvents.Hub.AddListener(LocalEvents.UserInterface.FinishGamePlay, FinishGame);
        LocalEvents.Hub.AddListener(LocalEvents.UserInterface.MainFormToClose, Close);
        LocalSaves.ReLocate();
    }

    private void KeyPressed(object? sender, KeyEventArgs e)
    {
        LocalEvents.Hub.Broadcast(LocalEvents.UserInterface.KeyPressed, e.KeyCode);
    }

    private void RelodeArchive(Archive archive)
    {
        Atlas.Relocate(archive);
        SpanFlow.Relocate(archive.Info.CurrentSpan);
        LocalEvents.Hub.Broadcast(LocalEvents.Flow.SwichFlowState);
        Controls.Clear();
        Controls.AddRange([
            Settings,
            ToolBar,
            Overview,
            GamePlane,
            InfoBar,
        ]);
        ArchiveSelector.DisableListener();
        Settings.EnableListener();
        DrawClient();
    }

    private void FinishGame()
    {

        Controls.Clear();
        Controls.Add(ArchiveSelector);
        Settings.DisableListener();
        ArchiveSelector.EnableListener();
        DrawClient();
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

    private void DrawClient()
    {
        if (Math.Min(ClientSize.Width, ClientSize.Height) is 0)
            return;
        LocalEvents.Hub.Broadcast(LocalEvents.UserInterface.MainFormOnDraw, ClientRectangle);

        //LocalEvents.ForTest();
    }
}
