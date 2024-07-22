using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.User;

namespace WarringStates.UI;

public partial class Selector : Displayer
{
    public ArchiveInfoList ArchiveInfoList { get; set; } = [];

    public override Size Padding { get; set; } = new(3, 3);

    public Color SelectedBarColor { get; set; } = Color.DarkSlateGray;

    int Offset { get; set; } = 0;

    int OffsetMax { get; set; } = 0;

    int LasOffsetMax { get; set; } = 0;
    
    Rectangle ItemColumnRect { get; set; } = new();

    List<Rectangle> ItemRects { get; } = [];

    Rectangle BarColumnRect { get; set; } = new();
    
    Rectangle BarRect { get; set; } = new();

    int BarHeight { get; set; } = 0;

    int BarHeightMin { get; set; } = 20;

    double BarRatio { get; set; } = 0;
    
    int ItemHeight { get; set; } = 125;

    int ItemShowCount { get; set; } = 0;

    public int SelectedIndex { get; private set; } = -1;

    FontData ItemFontData { get; set; } = new()
    {
        Size = 35f,
        Style = FontStyle.Bold,
    };

    public void ChangeOffset(int dOffset)
    {
        Offset += Dragger switch
        {
            DragPart.Item => -dOffset,
            DragPart.Bar => (dOffset * BarRatio).ToRoundInt(),
            _ => 0
        };
        Offset = Offset < 0 ? 0 : Offset > OffsetMax ? OffsetMax : Offset;
    }

    public override void Redraw()
    {
        using var g = Graphics.FromImage(Image);
        SetSize();
        g.Clear(FrontColor);
        var showStartItemIndex = Offset / ItemHeight;
        var top = ItemColumnRect.Top - (Offset % ItemHeight) + Padding.Height;
        using var brush = new SolidBrush(FrontColor);
        ItemRects.Clear();
        for (var i = 0; i < ItemShowCount; i++)
        {
            var y = top + i * ItemHeight;
            var rect = new Rectangle(ItemColumnRect.Left, y, ItemColumnRect.Width, ItemHeight - Padding.Height);
            var index = showStartItemIndex + i;
            if (!ArchiveInfoList.TryGetValue(index, out var info))
                continue;
            if (index == SelectedIndex)
                brush.Color = Color.Gold;
            else
                brush.Color = BackColor;
            g.FillRectangle(brush, rect);
            brush.Color = FrontColor;
            g.DrawString(info.WorldName, ItemFontData, brush, rect, new() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            ItemRects.Add(rect);
        }
        brush.Color = FrontColor;
        g.FillRectangle(brush, new(0, 0, Width, Padding.Height));
        g.FillRectangle(brush, new(0, Height - Padding.Height, Width, Padding.Height));
        var offset = ((Offset / BarRatio)).ToRoundInt();
        BarRect = new Rectangle(BarColumnRect.Left, BarColumnRect.Top + offset, BarColumnRect.Width, BarHeight);
        if (Dragger is DragPart.Bar)
        {
            brush.Color = SelectedBarColor;
            g.FillRectangle(brush, BarRect);
        }
        else
        {
            brush.Color = BackColor;
            g.FillRectangle(brush, BarRect);
        }
    }

    private void SetSize()
    {
        var height = Height - Padding.Height;
        var itemCount = ArchiveInfoList.Count;
        if (itemCount is 0)
            itemCount = 1;
        OffsetMax = Math.Max(0, itemCount * ItemHeight - height);
        if (LasOffsetMax is not 0)
            Offset = (OffsetMax / (double)LasOffsetMax * Offset).ToRoundInt();
        LasOffsetMax = OffsetMax;
        var barLineHeight = height - Padding.Height;
        BarRatio = (itemCount * ItemHeight) / (double)barLineHeight;
        if (BarRatio < 1)
            BarRatio = 1;
        BarHeight = (barLineHeight / BarRatio).ToRoundInt();
        if (BarHeight < BarHeightMin)
        {
            BarRatio = (itemCount * ItemHeight) / (double)(barLineHeight - (BarHeightMin - BarHeight));
            BarHeight = BarHeightMin;
        }
        ItemShowCount = height / ItemHeight;
        if (itemCount < ItemShowCount)
        {
            ItemShowCount = itemCount;
            height = ItemShowCount * ItemHeight;
        }
        else
        {
            if (height % ItemHeight is not 0)
                ItemShowCount++;
            if (Offset % ItemHeight is not 0)
                ItemShowCount++;
        }
        if (OffsetMax is 0)
        {
            ItemColumnRect = new(Padding.Width, 0, Width - Padding.Width * 2, height);
            BarColumnRect = new();
        }
        else
        {
            var barWidth = 30;
            ItemColumnRect = new(Padding.Width, 0, Width - barWidth - Padding.Width * 3, height);
            BarColumnRect = new(ItemColumnRect.Right + Padding.Width, Padding.Height, barWidth, barLineHeight);
        }
    }

    private void RollBarRedraw()
    {
        using var g = Graphics.FromImage(Image);
        if (Dragger is DragPart.Bar)
            g.FillRectangle(new SolidBrush(SelectedBarColor), BarRect);
        else
            g.FillRectangle(new SolidBrush(BackColor), BarRect);
    }
}
