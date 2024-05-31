using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Events;
using WarringStates.User;

namespace WarringStates.UI;

public partial class InitializeDisplayer : Displayer
{
    //int RollOffset { get; set; } = 0;

    //int RollOffsetMax { get; set; } = 0;

    //Rectangle RollRect { get; set; } = new();

    //Rectangle RollBarLineRect { get; set; } = new();

    //int RollBarHeight { get; set; } = 0;

    //int RollBarHeightMin { get; set; } = 20;

    //double RollBarRatio { get; set; }

    //double LastRollOffsetMax { get; set; } = 0;

    //Rectangle RollBarRect { get; set; } = new();

    RollViewer Roll { get; } = new();

    Rectangle InfoRect { get; set; } = new();

    //int ItemHeight { get; set; } = 125;

    new Size Padding { get; } = new(30, 30);

    //int ItemPadding { get; } = 3;

    //int ItemCount { get; set; } = 0;

    //int ItemToShowCount { get; set; }

    List<Archive> Archives { get; set; } = [];

    public InitializeDisplayer()
    {
        BackColor = Color.Teal;
        SizeChanged += OnResize;
    }

    private void OnResize(object? sender, EventArgs e)
    {
        Relocate();
        var width = Width - Padding.Width * 3;
        var height = Height - Padding.Height * 2;
        var colWidth = width / 3;
        using var g = Graphics.FromImage(Image);
        Roll.ItemCount = 55;
        Roll.ReSize(new Rectangle(Padding.Width, Padding.Height, colWidth * 2, height));
        g.FillRectangle(new SolidBrush(Color.White), Roll.Rect);
        Roll.ReDraw(g);
        //RollRect = new(Padding.Width, Padding.Height, colWidth * 2 - Padding.Width, height);
        //RollBarLineRect = new(RollRect.Right, RollRect.Top, Padding.Width, RollRect.Height);
        InfoRect = new(Roll.Rect.Right + Padding.Width, Padding.Height, colWidth, height);
        //ItemToShowCount = RollRect.Height / ItemHeight;
        ////ItemCount = Archives.Count + 1;
        //ItemCount = 200;
        ////ItemCount = 5;
        //RollOffsetMax = Math.Max(0, ItemCount * ItemHeight - RollRect.Height);
        //if (LastRollOffsetMax is not 0)
        //    RollOffset = (RollOffsetMax / (double)LastRollOffsetMax * RollOffset).ToRoundInt();
        //LastRollOffsetMax = RollOffsetMax;
        //RollBarRatio = (ItemCount * ItemHeight) / (double)RollRect.Height;
        //if (RollBarRatio < 1)
        //    RollBarRatio = 1;
        //RollBarHeight = (RollRect.Height / RollBarRatio).ToRoundInt();
        //RollOffset = (RollRect.Height * RollOffsetRatio).ToRoundInt();
        //RollOffsetRatio = RollOffset / (double)RollRect.Height;
        //using var g = Graphics.FromImage(Image);
        //g.FillRectangle(new SolidBrush(Color.White), new(RollRect.Left, RollRect.Top, RollRect.Width + RollBarLineRect.Width, RollRect.Height));
        g.FillRectangle(new SolidBrush(Color.White), new(InfoRect.Left, InfoRect.Top, InfoRect.Width, InfoRect.Height));
        //RollBarRect = new();
        //DrawArchives();
        Invalidate();
    }

    //private void DrawArchives()
    //{
    //    var showCount = ItemToShowCount;
    //    if (RollRect.Height % ItemHeight is not 0)
    //        showCount++;
    //    if (RollOffset % ItemHeight is not 0)
    //        showCount++;
    //    using var g = Graphics.FromImage(Image);
    //    var pen = new Pen(Color.White, 2f);
    //    g.FillRectangle(new SolidBrush(Color.White), RollRect);
    //    g.FillRectangle(new SolidBrush(Color.White), RollBarRect);
    //    //g.DrawRectangle(pen, RollRect);
    //    //g.FillRectangle(new SolidBrush(Color.White), new(RollRect.Right, RollRect.Top, Padding.Width, RollRect.Height));

    //    var offset = RollRect.Top - (RollOffset % ItemHeight);
    //    LocalEvents.Hub.Broadcast(LocalEvents.Test.AddSingleInfo, new TestForm.StringInfo("roll offset", RollOffset.ToString()));
    //    LocalEvents.Hub.Broadcast(LocalEvents.Test.AddSingleInfo, new TestForm.StringInfo("show count", showCount.ToString()));
    //    var left = RollRect.Left + ItemPadding;
    //    var top = offset + ItemPadding;
    //    var itemPaddings = ItemPadding * 2;
    //    var width = RollRect.Width - itemPaddings;
    //    var height = ItemHeight - itemPaddings;
    //    for (var i = 0; i < showCount; i++)
    //    {
    //        var y = top + i * ItemHeight;
    //        //g.DrawLine(new Pen(Color.White, 2.5f), new(RollRect.Left, y), new(RollRect.Right, y));
    //        var rect = new Rectangle(left, y, width, height);
    //        if (rect.CutRectInRange(new(RollRect.Left, RollRect.Top + ItemPadding, RollRect.Width, RollRect.Height - itemPaddings), out var r))
    //            g.FillRectangle(new SolidBrush(BackColor), r.Value);
    //    }
    //    var rollbarHeight = RollBarHeight;
    //    var rollBarRatio = RollBarRatio;
    //    if (RollBarHeight < RollBarHeightMin)
    //    {
    //        rollBarRatio = (ItemCount * ItemHeight) / (double)(RollRect.Height - (RollBarHeightMin - RollBarHeight));
    //        rollbarHeight = RollBarHeightMin;
    //    }
    //    var rollBarOffset = (RollOffset / rollBarRatio);
    //    RollBarRect = new Rectangle(RollRect.Right, (RollRect.Top + rollBarOffset).ToRoundInt(), Padding.Width - ItemPadding, rollbarHeight);
    //    g.FillRectangle(new SolidBrush(BackColor), RollBarRect);

    //    Invalidate();
    //}
}
