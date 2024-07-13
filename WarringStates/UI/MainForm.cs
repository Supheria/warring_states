//using LocalUtilities.SimpleScript.Serialization;
//using LocalUtilities.TypeGeneral;
//using WarringStates.Events;
//using WarringStates.Flow;
//using WarringStates.Graph;
//using WarringStates.UI.Component;
//using WarringStates.User;

//namespace WarringStates.UI;

//public partial class MainForm : ResizeableForm
//{
//    public override string LocalName => nameof(MainForm);

//    public override Size MinimumSize { get; set; } = new(200, 200);

//    Settings Settings { get; } = new();

//    ToolBar ToolBar { get; } = new();

//    GamePlane GamePlane { get; } = new();

//    Overview Overview { get; } = new();

//    InfoBar InfoBar { get; } = new();

//    LatticeGrid Grid { get; } = new();

//    //SpanFlow SpanFlow { get; set; } = new();

//    //AnimateFlow AnimateFlow { get; set; } = new();

//    public MainForm()
//    {
//        MinimumSize = new(860, 530);
//        OnDrawClient += DrawClient;
//        OnLoadForm += LoadForm;
//        OnSaveForm += SaveForm;
//        KeyDown += KeyPressed;
//        LocalEvents.Hub.TryAddListener(LocalEvents.UserInterface.StartGamePlay, StartGame);
//        LocalEvents.Hub.TryAddListener(LocalEvents.UserInterface.FinishGamePlay, FinishGame);
//        LocalEvents.Hub.TryAddListener(LocalEvents.UserInterface.MainFormToClose, Close);
//        ArchiveManager.ReLocate();
//    }

//    private void StartGame()
//    {
//        Controls.Clear();
//        Controls.AddRange([
//            Settings,
//            ToolBar,
//            Overview,
//            GamePlane,
//            InfoBar,
//        ]);
//        Settings.EnableListener();
//        DrawClient();
//    }

//    private void KeyPressed(object? sender, KeyEventArgs e)
//    {
//        Controls.Clear();
//        Controls.AddRange([
//            Settings,
//            ToolBar,
//            Overview,
//            GamePlane,
//            InfoBar,
//        ]);
//        DrawClient();
//    }

//    private void FinishGame()
//    {
//        Controls.Clear();
//        Settings.DisableListener();
//        DrawClient();
//    }

//    private void SaveForm(SsSerializer serializer)
//    {
//        //serializer.WriteTag(nameof(SpanFlow.CurrentSpan), SpanFlow.CurrentSpan.ToString());
//        //serializer.WriteObject(Grid);
//    }

//    private void LoadForm(SsDeserializer deserializer)
//    {
//        var startSpan = deserializer.ReadTag(nameof(SpanFlow.CurrentSpan), int.Parse);
//        //SpanFlow.Relocate(startSpan);
//        //deserializer.ReadObject(Grid);
//    }

//    private void DrawClient()
//    {
//        if (Math.Min(ClientSize.Width, ClientSize.Height) is 0)
//            return;
//        LocalEvents.Hub.TryBroadcast(LocalEvents.UserInterface.MainFormOnDraw, ClientRectangle);

//        //LocalEvents.ForTest();
//    }
//}
