using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.EventProcess;
using LocalUtilities.TypeToolKit.Graph;
using LocalUtilities.TypeToolKit.Mathematic;
using System;

namespace WarringStates;

public class OverviewDisplayer : Displayer, IEventListener
{
    bool FullScreen { get; set; } = false;

    public void EnableListener()
    {
        EventManager.Instance.AddEvent(LocalEventId.GameFormUpdate, this);
        EventManager.Instance.AddEvent(LocalEventId.GridUpdate, this);
    }

    public void HandleEvent(int eventId, IEventArgument argument)
    {
        if (eventId is LocalEventId.GameFormUpdate)
        {
            if (argument is not GameFormUpdateEventArgument arg)
                return;
            if (Terrain.Overview is null)
                return;
            var range = arg.ClientSize;
            var size = FullScreen ? range : new((int)(range.Width * 0.25), (int)(range.Height * 0.25));
            size = Terrain.Overview.Size.ScaleSizeOnRatio(size);
            Location = FullScreen ? new((range - size) / 2) : new(range.Width - size.Width, 0);
            Size = size;
        }
        else if (eventId is LocalEventId.GridUpdate)
        {
            if (argument is not GridUpdatedEventArgument arg)
                return;
            Relocate(arg);
        }
    }

    private void Relocate(GridUpdatedEventArgument arg)
    {
        if (Width is 0 || Height is 0 || Terrain.Overview is null) 
            return;
        Relocate();
        Image = Terrain.Overview.CopyToNewSize(Size);
        var edgeLength = LatticeCell.CellData.EdgeLength;
        var width = arg.DrawRect.Width / edgeLength;
        var height = arg.DrawRect.Height / edgeLength;
        var x = Terrain.Width - arg.Origin.X / (double)edgeLength/* - width / 2*/;
        var y = Terrain.Height - arg.Origin.Y / (double)edgeLength/* - height / 2*/;
        var widthRatio = Terrain.Width / (double)Width;
        var heightRatio = Terrain.Height / (double)Height;
        var rect = new Rectangle((x / widthRatio).ToInt(), (y / heightRatio).ToInt(), (width / widthRatio).ToInt(), (height / heightRatio).ToInt());
        var g = Graphics.FromImage(Image);
        //var edges = rect.CutRectLoopEdgesInRange(new(new(0, 0), Size));
        //foreach (var edge in edges)
        //    g.DrawLine(Pens.Red, edge.Starter, edge.Ender);
        //g.DrawRectangle(Pens.Red, rect);
        var rects = rect.CutRectLoopRectsInRange(new(new(0, 0), Size));
        rects.ForEach(r => g.DrawRectangle(Pens.Red, r));
        g.Flush();
        g.Dispose();
        Invalidate();
    }
}
