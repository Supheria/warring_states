using LocalUtilities.TypeToolKit.Mathematic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Events;

namespace WarringStates.UI;

partial class InitializeDisplayer
{
    private class RollViewer
    {
        public int Offset { get; set; } = 0;

        int OffsetMax { get; set; } = 0;

        int LastOffsetMax { get; set; } = 0;

        public Rectangle Rect { get; private set; } = new();

        public Rectangle ItemsRect { get; private set; } = new();

        Rectangle BarLineRect { get; set; } = new();

        public Rectangle BarRect { get; private set; } = new();

        int BarWidth { get; set; } = 30;

        int BarHeight { get; set; } = 0;

        int BarHeightMin { get; set; } = 20;

        double BarRatio { get; set; } = 0;

        public int ItemHeight { get; private set; } = 125;

        int Padding { get; set; } = 3;

        //int DoublePadding { get; set; }

        public int ItemCount { get; set; } = 0;

        public int ItemToShowCount { get; private set; } = 0;

        static SolidBrush BackBrush { get; set; } = new(Color.White);

        static SolidBrush FrontBrush { get; set; } = new(Color.Teal);

        public void ReSize(Rectangle rect)
        {
            Rect = rect;
            var top = rect.Top;
            var height = rect.Height - Padding;
            ItemsRect = new(rect.Left + Padding, top, rect.Width - BarWidth - Padding * 3, height);
            BarLineRect = new(rect.Right - BarWidth - Padding, top + Padding, BarWidth, height - Padding);
            ItemToShowCount = ItemsRect.Height / ItemHeight;
            OffsetMax = Math.Max(0, ItemCount * ItemHeight - ItemsRect.Height);
            if (LastOffsetMax is not 0)
                Offset = (OffsetMax / (double)LastOffsetMax * Offset).ToRoundInt();
            LastOffsetMax = OffsetMax;
            BarRatio = (ItemCount * ItemHeight) / (double)BarLineRect.Height;
            if (BarRatio < 1)
                BarRatio = 1;
            BarHeight = (BarLineRect.Height / BarRatio).ToRoundInt();
            BarRect = new();
        }

        public void Relocate(int dOffset)
        {
            Offset += dOffset;
            Offset = Offset < 0 ? 0 : Offset > OffsetMax ? OffsetMax : Offset;
            LocalEvents.Hub.Broadcast(LocalEvents.Test.AddSingleInfo, new TestForm.StringInfo("roll offset", Offset.ToString()));
            //    
        }

        public void ReDraw(Graphics g)
        {
            g.FillRectangle(BackBrush, Rect);
            var showCount = ItemToShowCount;
            if (ItemsRect.Height % ItemHeight is not 0)
                showCount++;
            if (Offset % ItemHeight is not 0)
                showCount++;
            //var left = ItemsRect.Left + Padding;
            var top = ItemsRect.Top - (Offset % ItemHeight) + Padding;
            LocalEvents.Hub.Broadcast(LocalEvents.Test.AddSingleInfo, new TestForm.StringInfo("top", top.ToString()));
            //var width = ItemsRect.Width - BarWidth - Padding;
            //var height = ItemHeight - Padding;
            for (var i = 0; i< showCount; i++)
            {
                var y = top + i * ItemHeight;
                var rect = new Rectangle(ItemsRect.Left, y, ItemsRect.Width, ItemHeight - Padding);
                DrawItem(g, rect);
                //if (rect.CutRectInRange(ItemsRect, out var r))
                //DrawItem(g, r.Value);
            }
            g.FillRectangle(BackBrush, new(ItemsRect.Left, ItemsRect.Top, ItemsRect.Width, Padding));
            g.FillRectangle(BackBrush, new(ItemsRect.Left, ItemsRect.Bottom, ItemsRect.Width, Padding));
            var barHeight = BarHeight;
            var barRatio = BarRatio;
            if (barHeight < BarHeightMin)
            {
                barRatio = (ItemCount * ItemHeight) / (double)(BarLineRect.Height - (BarHeightMin - barHeight));
                barHeight = BarHeightMin;
            }
            var offset = ((Offset / barRatio)).ToRoundInt();
            BarRect = new Rectangle(BarLineRect.Left, BarLineRect.Top + offset, BarLineRect.Width, barHeight);
            g.FillRectangle(FrontBrush, BarRect);
        }

        private void DrawItem(Graphics g, Rectangle rect)
        {
            g.FillRectangle(FrontBrush, rect);
        }
    }
}
