using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Graph;
using LocalUtilities.TypeToolKit.Mathematic;
using System.Drawing;
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

    int BarHeight { get; set; } = 0;

    int BarHeightMin { get; set; } = 20;

    double BarRatio { get; set; } = 0;

    int RollItemHeight { get; set; } = 125;

    int RollPadding { get; set; } = 3;

    int RollItemToShowCount { get; set; } = 0;

    Rectangle SelectedItemRect { get; set; } = new();

    int SelectedItemIndex { get; set; } = -1;

    FontData ItemFontData { get; set; } = new(nameof(ItemFontData))
    {
        Size = 35f,
        Style = FontStyle.Bold,
    };

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

    private void RollReDraw()
    {
        RollReSize();
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
            if (!LocalSaves.TryGetArchiveInfo(index, out var info))
                continue;
            if (index == SelectedItemIndex)
                brush.Color = Color.Gold;
            else
                brush.Color = BackColor;
            g.FillRectangle(brush, rect);
            brush.Color = FrontColor;
            g.DrawString(info.WorldName, ItemFontData, brush, rect, new() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        }
        brush.Color = FrontColor;
        g.FillRectangle(brush, new(RollItemsRect.Left, RollItemsRect.Top, RollItemsRect.Width, RollPadding));
        g.FillRectangle(brush, new(RollItemsRect.Left, RollItemsRect.Bottom, RollItemsRect.Width, RollPadding));
        brush.Color = BackColor;
        g.FillRectangle(brush, new(RollItemsRect.Left, Top, RollItemsRect.Width, Padding.Height));
        g.FillRectangle(brush, new(RollItemsRect.Left, RollItemsRect.Bottom + RollPadding, RollItemsRect.Width, Padding.Height));
        var offset = ((RollOffset / BarRatio)).ToRoundInt();
        BarRect = new Rectangle(BarLineRect.Left, BarLineRect.Top + offset, BarLineRect.Width, BarHeight);
        RollBarRedraw();
    }

    private void RollReSize()
    {
        var top = RollRect.Top;
        var height = RollRect.Height - RollPadding;
        var itemCount = LocalSaves.Count;
        if (itemCount is 0)
            itemCount = 1;
        RollOffsetMax = Math.Max(0, itemCount * RollItemHeight - height);
        if (LastRollOffsetMax is not 0)
            RollOffset = (RollOffsetMax / (double)LastRollOffsetMax * RollOffset).ToRoundInt();
        LastRollOffsetMax = RollOffsetMax;
        var barLineHeight = height - RollPadding;
        BarRatio = (itemCount * RollItemHeight) / (double)barLineHeight;
        if (BarRatio < 1)
            BarRatio = 1;
        BarHeight = (barLineHeight / BarRatio).ToRoundInt();
        if (BarHeight < BarHeightMin)
        {
            BarRatio = (itemCount * RollItemHeight) / (double)(barLineHeight - (BarHeightMin - BarHeight));
            BarHeight = BarHeightMin;
        }
        if (RollOffsetMax is 0)
        {
            RollItemsRect = new(RollRect.Left + RollPadding, top, RollRect.Width - RollPadding * 2, height);
            BarLineRect = new();
        }
        else
        {
            var barWidth = 30;
            RollItemsRect = new(RollRect.Left + RollPadding, top, RollRect.Width - barWidth - RollPadding * 3, height);
            BarLineRect = new(RollRect.Right - barWidth - RollPadding, top + RollPadding, barWidth, barLineHeight);
        }
        RollItemToShowCount = RollItemsRect.Height / RollItemHeight;
    }

    private void RollBarRedraw()
    {
        using var g = Graphics.FromImage(Image);
        if (RollDragger is RollDragPart.Bar)
            g.FillRectangle(new SolidBrush(Color.DarkSlateGray), BarRect);
        else
            g.FillRectangle(new SolidBrush(BackColor), BarRect);
        Invalidate();
    }
}
