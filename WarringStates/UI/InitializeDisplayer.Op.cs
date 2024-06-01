using LocalUtilities.TypeGeneral;

namespace WarringStates.UI;

partial class InitializeDisplayer
{
    protected override void AddOperations()
    {
        MouseDown += OnMouseDown;
        MouseUp += OnMouseUp;
        MouseMove += OnMouseMove;
        MouseDoubleClick += OnDoubleClick;
    }

    private void OnDoubleClick(object? sender, MouseEventArgs e)
    {
        if (RollItemsRect.Contains(e.Location))
            SelectedItemIndex = (e.Y - RollItemsRect.Top - RollPadding + RollOffset) / RollItemHeight;
        else
            SelectedItemIndex = -1;
        RollReDraw();
    }

    private void OnMouseDown(object? sender, MouseEventArgs e)
    {
        if (RollItemsRect.Contains(e.Location))
            RollDragger = RollDragPart.Item;
        else if (BarRect.Contains(e.Location))
            RollDragger = RollDragPart.Bar;
        else if (StartButton.Rect.Contains(e.Location))
        {

        }
        else if (BuildButton.Rect.Contains(e.Location))
        {

        }
        else if (DeleteButton.Rect.Contains(e.Location) && SelectedItemIndex is not -1)
        {

        }
    }

    private void OnMouseUp(object? sender, MouseEventArgs e)
    {
        RollDragger = RollDragPart.None;
    }

    private void OnMouseMove(object? sender, MouseEventArgs e)
    {
        switch (DragFlag)
        {
            case Directions.Left:
                RollChangeOffset(e.Y - DragStartPoint.Y);
                break;
        }
        if (RollDragger is RollDragPart.None)
        {
            testButton(StartButton);
            testButton(BuildButton);
            testButton(DeleteButton);
        }
        void testButton(Button button)
        {
            if (button.Rect.Contains(e.Location))
            {
                button.Selected = true;
                ButtonRedraw(button);
            }
            else if (button.Selected)
            {
                button.Selected = false;
                ButtonRedraw(button);
            }
        }
    }
}
