using AltitudeMapGenerator;
using AltitudeMapGenerator.Layout;
using WarringStates.Client;
using WarringStates.Client.User;
using WarringStates.User;

namespace WarringStates.Server.Component;

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
            //var data = new AltitudeMapData(new(1000, 1000), new(8, 8), new(8, 8), RiverLayout.Types.ForwardSlash, 7, 650000, 0.75f);
            //var data = new AltitudeMapData(new(500, 500), new(5, 5), new(8, 8), RiverLayout.Types.Horizontal, 2.25, 180000, 0.66f);
            //var data = new AltitudeMapData(new(300, 300), new(3, 3), new(8, 8), RiverLayout.Types.Horizontal, 2.25, 60000, 0.66f);
            //data.CreateArchive("new world");
            //SelectedItemIndex = 0;
            //RollReDraw();
            //ThumbnailRedraw();
            LocalEvents.Hub.TryBroadcast(LocalEvents.UserInterface.ArchiveListToRelocate);
        }
        else if (JoinButton.Rect.Contains(e.Location) && LocalArchives.TryGetArchiveInfo(SelectedItemIndex, out var archive))
        {
            //LocalEvents.Hub.TryBroadcast(LocalEvents.UserInterface.ArchiveSelected, archive);
            //ArchiveManager.Update(SelectedItemIndex);
            //SelectedItemIndex = 0;
            //RollReDraw();
            //ThumbnailRedraw();
        }
        //else if (DeleteButton.Rect.Contains(e.Location) && ArchiveManager.Delete(SelectedItemIndex))
        //{
        //    if (ArchiveManager.Count is 0)
        //        SelectedItemIndex = -1;
        //    else if (SelectedItemIndex >= ArchiveManager.Count)
        //        SelectedItemIndex--;
        //    RollReDraw();
        //    ButtonRedraw(DeleteButton, false);
        //    ThumbnailRedraw();
        //}
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
            //testButton(DeleteButton, () => SelectedItemIndex is not -1);
            //LocalEvents.Hub.Broadcast(LocalEvents.Test.AddSingleInfo, new TestForm.StringInfo("SelectedItemIndex", SelectedItemIndex.ToString()));
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
