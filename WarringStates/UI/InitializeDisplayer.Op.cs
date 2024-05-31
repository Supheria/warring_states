using LocalUtilities.TypeGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

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
        if (Roll.ItemsRect.Contains(e.Location))
            DoDrawRoll = true;
        else if (Roll.BarRect.Contains(e.Location))
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
                    Roll.Relocate(DragStartPoint.Y - e.Y);
                    using var g = Graphics.FromImage(Image);
                    Roll.ReDraw(g);
                    //RollOffset += (DragStartPoint.Y - e.Y);
                    //RollOffset = RollOffset < 0 ? 0 : RollOffset > RollOffsetMax ? RollOffsetMax : RollOffset;
                    //DrawArchives();
                    Invalidate();
                }
                else if (DoDrawBar)
                {
                    Roll.Relocate((e.Y - DragStartPoint.Y) * Roll.ItemHeight * Roll.ItemToShowCount / Roll.BarRect.Height);
                    using var g = Graphics.FromImage(Image);
                    Roll.ReDraw(g);
                    //RollOffset -= (DragStartPoint.Y - e.Y) * ItemHeight * ItemToShowCount / (RollBarRect.Height);
                    //RollOffset = RollOffset < 0 ? 0 : RollOffset > RollOffsetMax ? RollOffsetMax : RollOffset;
                    //DrawArchives();
                    Invalidate();
                }
                break;
        }
    }
}
