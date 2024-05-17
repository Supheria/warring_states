using LocalUtilities.TypeGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarringStates;

internal class GameForm : ResizeableForm<GameFormData>
{
    GameDisplayer Displayer { get; } = new();

    protected override void InitializeComponent()
    {
        OnDrawingClient += DrawClient;
        Controls.Add(Displayer);
    }

    private void DrawClient()
    {
        if(Math.Min(ClientRectangle.Width, ClientRectangle.Height) <= 0)
            return;
        Displayer.Bounds = new(Left, Top, Width, Height);
        Displayer.ResetImage();
    }
}
