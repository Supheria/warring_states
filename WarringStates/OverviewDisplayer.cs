using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.EventProcess;
using LocalUtilities.TypeToolKit.Graph;
using LocalUtilities.TypeToolKit.Mathematic;

namespace WarringStates;

public class OverviewDisplayer : Displayer, IEventListener
{
    bool FullScreen { get; set; } = false;

    public OverviewDisplayer()
    {
        EventManager.Instance.AddEvent(LocalEventId.GridUpdate, this);
    }

    public void HandleEvent(int eventId, IEventArgument argument)
    {
        if (eventId is LocalEventId.GridUpdate)
        {
            if (argument is not GridUpdatedEventArgument arg)
                return;
            Relocate(arg);
        }
    }

    protected override bool OnSetRange(Size range)
    {
        if (Terrain.Overview is null)
        {
            Size = new();
            return false;
        }
        var size = FullScreen ? range : new((int)(range.Width * 0.25), (int)(range.Height * 0.25));
        size = Terrain.Overview.Size.ScaleSizeOnRatio(size);
        Location = FullScreen ? new((range - size) / 2) : new(range.Width - size.Width, 0);
        Size = size;
        return true;
    }

    protected override void Relocate()
    {
        //Image = Terrain.Overview?.CopyToNewSize(Size);
        //Invalidate();
    }

    private void Relocate(GridUpdatedEventArgument arg)
    {
        //Relocate();
        Image = Terrain.Overview?.CopyToNewSize(Size);
        var edgeLength = LatticeCell.CellData.EdgeLength;
        var widthRatio = Terrain.Width / (double)Width;
        var heightRatio = Terrain.Height / (double)Height;
        var width = arg.DrawRect.Width / edgeLength;
        var height = arg.DrawRect.Height / edgeLength;
        var x = Terrain.Width - arg.Origin.X / (double)edgeLength/* - width / 2*/;
        var y = Terrain.Height - arg.Origin.Y / (double)edgeLength/* - height / 2*/;
        var rect = new Rectangle((x / widthRatio).ToInt(), (y / heightRatio).ToInt(), (width / widthRatio).ToInt(), (height / heightRatio).ToInt());
        var g = Graphics.FromImage(Image);
        var testInfo = $"{arg.Origin / edgeLength} => {Math.Round(x, 0)},{Math.Round(y, 0)}";
        EventManager.Instance.Dispatch(LocalEventId.TestInfo, new TestForm.TestInfo("origin", testInfo));
        testInfo = $"{Width},{Height}";
        EventManager.Instance.Dispatch(LocalEventId.TestInfo, new TestForm.TestInfo("size", testInfo));
        EventManager.Instance.Dispatch(LocalEventId.TestInfo, new TestForm.TestInfo("col row", $"{width},{height}"));
        EventManager.Instance.Dispatch(LocalEventId.TestInfo, new TestForm.TestInfo("rect", rect.ToString()));
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
