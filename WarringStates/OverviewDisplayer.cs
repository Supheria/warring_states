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
        Invalidate();
    }

    private void Relocate(GridUpdatedEventArgument arg)
    {
        //Relocate();
        Image = Terrain.Overview?.CopyToNewSize(Size);
        var edgeLength = LatticeCell.CellData.EdgeLength;
        var width = arg.DrawRect.Width / edgeLength;
        var height = arg.DrawRect.Height / edgeLength;
        var widthRatio = Terrain.Width / (double)Width;
        var heightRatio = Terrain.Height / (double)Height;
        width = (int)(width * widthRatio);
        height = (int)(height * heightRatio);
        var x = width / (double)arg.DrawRect.Width * arg.DrawRect.X;
        var y = height / (double)arg.DrawRect.Height * arg.DrawRect.Y;
        var g = Graphics.FromImage(Image);
        g.DrawRectangle(Pens.Red, new((int)x, (int)y, width, height));
        g.Flush();
        g.Dispose();
        Invalidate();
    }
}
