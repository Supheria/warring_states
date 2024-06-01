using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using WarringStates.Events;

namespace WarringStates.UI;

internal class InitializeForm : ResizeableForm
{
    public override string LocalName => nameof(InitializeForm);

    InitializeDisplayer Displayer { get; } = new();

    protected override void InitializeComponent()
    {
        Controls.AddRange([
            Displayer,
            ]);
        OnDrawingClient += DrawClient;
        OnLoadForm += LoadForm;
    }

    private void LoadForm(SsDeserializer deserializer)
    {
        LocalEvents.Hub.Broadcast(LocalEvents.UserInterface.InitializeFormLoading);
    }

    private void DrawClient()
    {
        Displayer.Bounds = ClientRectangle;
    }
}
