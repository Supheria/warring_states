using LocalUtilities.TypeGeneral;
using WarringStates.User;

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
        else if (BuildButton.Rect.Contains(e.Location))
        {

        }
        else if (LoadButton.Rect.Contains(e.Location) && SelectedItemIndex is not -1)
        {
            if (LocalSaves.LoadArchive(LocalSaves.Saves[SelectedItemIndex], out var archive))
            {

            }
        }
        else if (DeleteButton.Rect.Contains(e.Location) && SelectedItemIndex is not -1)
        {
            LocalSaves.Delete(LocalSaves.Saves[SelectedItemIndex]);
            SelectedItemIndex = -1;
            RollReDraw();
            ButtonRedraw(DeleteButton, false);
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
            testButton(BuildButton, () => true);
            testButton(LoadButton, () => SelectedItemIndex is not -1);
            testButton(DeleteButton, () => SelectedItemIndex is not -1);
        }
        void testButton(Button button, Func<bool> condition)
        {
            if (button.Rect.Contains(e.Location) && condition())
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
