using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Graph;
using LocalUtilities.TypeToolKit.Mathematic;
using WarringStates.Events;
using WarringStates.User;

namespace WarringStates.UI;

partial class InitializeDisplayer
{
    enum RollDragPart
    {
        None,
        Item,
        Bar
    }
    RollDragPart RollDragger { get; set; } = RollDragPart.None;

    int RollOffset { get; set; } = 0;

    int RollOffsetMax { get; set; } = 0;

    int LastRollOffsetMax { get; set; } = 0;

    Rectangle RollRect { get; set; } = new();

    Rectangle RollItemsRect { get; set; } = new();

    Rectangle BarLineRect { get; set; } = new();

    Rectangle BarRect { get; set; } = new();

    int BarWidth { get; set; } = 30;

    int BarHeight { get; set; } = 0;

    int BarHeightMin { get; set; } = 20;

    double BarRatio { get; set; } = 0;

    int RollItemHeight { get; set; } = 125;

    int RollPadding { get; set; } = 3;

    int RollItemToShowCount { get; set; } = 0;

    int SelectedItemIndex { get; set; } = -1;

    Rectangle SelectedItemRect { get; set; } = new();

    FontData ItemFontData = new(nameof(ItemFontData))
    {
        Size = 35f,
        Style = FontStyle.Bold,
    };

    private void RollReSize()
    {
        var top = RollRect.Top;
        var height = RollRect.Height - RollPadding;
        RollItemsRect = new(RollRect.Left + RollPadding, top, RollRect.Width - BarWidth - RollPadding * 3, height);
        BarLineRect = new(RollRect.Right - BarWidth - RollPadding, top + RollPadding, BarWidth, height - RollPadding);
        RollItemToShowCount = RollItemsRect.Height / RollItemHeight;
        RollReDraw();
    }

    private void RollChangeOffset(int dOffset)
    {
        RollOffset += RollDragger switch
        {
            RollDragPart.Item => -dOffset,
            RollDragPart.Bar => (dOffset * BarRatio).ToRoundInt(),
            _ => 0
        };
        RollOffset = RollOffset < 0 ? 0 : RollOffset > RollOffsetMax ? RollOffsetMax : RollOffset;
        RollReDraw();
    }

    private void UpdateItemCount()
    {
        var itemCount = LocalSaves.Saves.Count;
        if (itemCount is 0)
            itemCount = 1;
        RollOffsetMax = Math.Max(0, itemCount * RollItemHeight - RollItemsRect.Height);
        if (LastRollOffsetMax is not 0)
            RollOffset = (RollOffsetMax / (double)LastRollOffsetMax * RollOffset).ToRoundInt();
        LastRollOffsetMax = RollOffsetMax;
        BarRatio = (itemCount * RollItemHeight) / (double)BarLineRect.Height;
        if (BarRatio < 1)
            BarRatio = 1;
        BarHeight = (BarLineRect.Height / BarRatio).ToRoundInt();
        if (BarHeight < BarHeightMin)
        {
            BarRatio = (itemCount * RollItemHeight) / (double)(BarLineRect.Height - (BarHeightMin - BarHeight));
            BarHeight = BarHeightMin;
        }
    }

    private void RollReDraw()
    {
        UpdateItemCount();
        using var g = Graphics.FromImage(Image);
        using var brush = new SolidBrush(FrontColor);
        g.FillRectangle(brush, RollRect);
        var showCount = RollItemToShowCount;
        if (RollItemsRect.Height % RollItemHeight is not 0)
            showCount++;
        if (RollOffset % RollItemHeight is not 0)
            showCount++;
        var showStartItemIndex = RollOffset / RollItemHeight;
        var top = RollItemsRect.Top - (RollOffset % RollItemHeight) + RollPadding;
        LocalEvents.Hub.Broadcast(LocalEvents.Test.AddSingleInfo, new TestForm.StringInfo("showStartItemIndex", showStartItemIndex.ToString()));
        for (var i = 0; i < showCount; i++)
        {
            var y = top + i * RollItemHeight;
            var rect = new Rectangle(RollItemsRect.Left, y, RollItemsRect.Width, RollItemHeight - RollPadding);
            var index = showStartItemIndex + i;
            if (index >= LocalSaves.Saves.Count)
                continue;
            var info = LocalSaves.Saves[index];
            var text = $"{info.WorldName}";

            if (index == SelectedItemIndex)
                brush.Color = Color.Gold;
            else
                brush.Color = BackColor;
            g.FillRectangle(brush, rect);
            brush.Color = FrontColor;
            g.DrawString(text, ItemFontData, brush, rect, new() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        }
        brush.Color = FrontColor;
        g.FillRectangle(brush, new(RollItemsRect.Left, RollItemsRect.Top, RollItemsRect.Width, RollPadding));
        g.FillRectangle(brush, new(RollItemsRect.Left, RollItemsRect.Bottom, RollItemsRect.Width, RollPadding));
        brush.Color = BackColor;
        g.FillRectangle(brush, new(RollItemsRect.Left, Top, RollItemsRect.Width, Padding.Height));
        g.FillRectangle(brush, new(RollItemsRect.Left, RollItemsRect.Bottom + RollPadding, RollItemsRect.Width, Padding.Height));
        var offset = ((RollOffset / BarRatio)).ToRoundInt();
        BarRect = new Rectangle(BarLineRect.Left, BarLineRect.Top + offset, BarLineRect.Width, BarHeight);
        g.FillRectangle(brush, BarRect);
        Invalidate();
    }
}
