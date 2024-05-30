using LocalUtilities.TypeGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarringStates.UI;

public class InitializeDisplayer : Displayer
{
    public InitializeDisplayer()
    {
        SizeChanged += OnResize;
    }

    private void OnResize(object? sender, EventArgs e)
    {
        Relocate();

    }
}
