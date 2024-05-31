using LocalUtilities.TypeGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarringStates.UI;

partial class InitializeDisplayer
{
    bool DoDrawBar { get; set; } = false;

    bool DoDrawRoll { get; set; } = false;

    protected override void AddOperations()
    {
        MouseDown += OnMouseDown;
        MouseUp += OnMouseUp;
        MouseMove += OnMouseMove;
    }

    private void OnMouseDown(object? sender, MouseEventArgs e)
    {
        if (RollRect.Contains(e.Location))
            DoDrawRoll = true;
        else if (RollBarRect.Contains(e.Location))
            DoDrawBar = true;
    }

    private void OnMouseUp(object? sender, MouseEventArgs e)
    {
        if (DoDrawRoll)
            DoDrawRoll = false;
        if (DoDrawBar)
            DoDrawBar = false;
    }

    private void OnMouseMove(object? sender, MouseEventArgs e)
    {
        switch (DragFlag)
        {
            case Directions.Left:
                if (DoDrawRoll)
                {
                    RollOffset += (DragStartPoint.Y - e.Y);
                    RollOffset = RollOffset < 0 ? 0 : RollOffset > RollOffsetMax ? RollOffsetMax : RollOffset;
                    DrawArchives();
                }
                else if (DoDrawBar)
                {
                    RollOffset -= (DragStartPoint.Y - e.Y) * ItemHeight * ItemToShowCount / (RollBarRect.Height);
                    RollOffset = RollOffset < 0 ? 0 : RollOffset > RollOffsetMax ? RollOffsetMax : RollOffset;
                    DrawArchives();
                }
                break;
        }
    }
}
