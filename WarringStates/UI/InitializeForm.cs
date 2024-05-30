using LocalUtilities.TypeGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

    private void DrawClient()
    {
        Displayer.Bounds = ClientRectangle;
    }
}
