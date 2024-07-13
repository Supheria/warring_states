using WarringStates.Client.Events;
using WarringStates.Client.User;

namespace WarringStates.Client.UI.Component;

partial class ArchiveSelector
{
    Point DragStartPoint { get; set; } = new();

    private void AddOperations()
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
        ThumbnailRedraw();
    }

    private void OnMouseDown(object? sender, MouseEventArgs e)
    {
        if (RollItemsRect.Contains(e.Location))
            RollDragger = RollDragPart.Item;
        else if (BarRect.Contains(e.Location))
        {
            RollDragger = RollDragPart.Bar;
            RollBarRedraw();
        }
        else if (RefreshButton.Rect.Contains(e.Location))
        {
            LocalEvents.TryBroadcast(LocalEvents.UserInterface.FetchArchiveList);
        }
        else if (JoinButton.Rect.Contains(e.Location) && LocalArchives.TryGetArchiveInfo(SelectedItemIndex, out var archive))
        {

        }
        else if (LogoutButton.Rect.Contains(e.Location))
        {
            LocalEvents.TryBroadcast(LocalEvents.UserInterface.LogoutPlayer);
        }
    }

    private void OnMouseUp(object? sender, MouseEventArgs e)
    {
        RollDragger = RollDragPart.None;
        RollBarRedraw();
    }

    private void OnMouseMove(object? sender, MouseEventArgs e)
    {
        if (RollDragger is RollDragPart.None)
        {
            testButton(RefreshButton, () => true);
            testButton(JoinButton, () => SelectedItemIndex is not -1);
            testButton(LogoutButton, () => true);
            //LocalEvents.Broadcast(LocalEvents.Test.AddSingleInfo, new TestForm.StringInfo("SelectedItemIndex", SelectedItemIndex.ToString()));
        }
        else
            RollChangeOffset(e.Y - DragStartPoint.Y);
        void testButton(Button button, Func<bool> condition)
        {
            if (button.Rect.Contains(e.Location) && condition())
            {
                if (button.Selected)
                    return;
                button.Selected = true;
                ButtonRedraw(button);
            }
            else if (button.Selected)
            {
                button.Selected = false;
                ButtonRedraw(button);
            }
        }
        DragStartPoint = e.Location;
    }
}
