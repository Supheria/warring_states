﻿using AltitudeMapGenerator;
using AltitudeMapGenerator.Layout;
using LocalUtilities.TypeGeneral;
using WarringStates.Events;
using WarringStates.Map;
using WarringStates.User;

namespace WarringStates.UI.Component;

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
        else if (BuildButton.Rect.Contains(e.Location))
        {
            var data = new AltitudeMapData(new(1000, 1000), new(8, 8), new(8, 8), RiverLayout.Types.ForwardSlash, 7, 650000, 0.75f);
            //var data = new AltitudeMapData(new(500, 500), new(5, 5), new(8, 8), RiverLayout.Types.Horizontal, 2.25, 180000, 0.66f);
            data.CreateArchive("new world");
            SelectedItemIndex = 0;
            RollReDraw();
            ThumbnailRedraw();
        }
        else if (LoadButton.Rect.Contains(e.Location) && LocalSaves.LoadArchive(SelectedItemIndex, out var archive))
        {
            LocalEvents.Hub.Broadcast(LocalEvents.UserInterface.ArchiveSelected, archive);
            LocalSaves.Update(SelectedItemIndex);
            SelectedItemIndex = 0;
            RollReDraw();
            ThumbnailRedraw();
        }
        else if (DeleteButton.Rect.Contains(e.Location) && LocalSaves.Delete(SelectedItemIndex))
        {
            if (LocalSaves.Count is 0)
                SelectedItemIndex = -1;
            else if (SelectedItemIndex >= LocalSaves.Count)
                SelectedItemIndex--;
            RollReDraw();
            ButtonRedraw(DeleteButton, false);
            ThumbnailRedraw();
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
            testButton(BuildButton, () => true);
            testButton(LoadButton, () => SelectedItemIndex is not -1);
            testButton(DeleteButton, () => SelectedItemIndex is not -1);
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
