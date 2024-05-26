using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Graph;
using LocalUtilities.TypeToolKit.Mathematic;
using WarringStates.Events;
using WarringStates.Graph;
using WarringStates.Map;

namespace WarringStates.UI.Component;

public class OverviewDisplayer : Displayer
{
    bool FullScreen { get; set; } = false;

    public void EnableListener()
    {
        LocalEvents.Hub.AddListener<GameFormUpdateArgs>(LocalEvents.UserInterface.GameFormUpdate, SetBounds);
        LocalEvents.Hub.AddListener<GridUpdatedArgs>(LocalEvents.Graph.GridUpdate, Relocate);
    }

    private void SetBounds(GameFormUpdateArgs args)
    {
        if (Terrain.Overview is null)
            Size = new();
        else
        {
            var size = FullScreen ? args.GameRect.Size : new((int)(args.GameRect.Width * 0.25), (int)(args.GameRect.Height * 0.25));
            Size = Terrain.Overview.Size.ScaleSizeOnRatio(size);
            Location = FullScreen ? new((args.GameRect.Size - Size) / 2) : new(args.GameRect.Right - Width, args.GameRect.Top);
        }
    }

    private void Relocate(GridUpdatedArgs args)
    {
        if (Width is 0 || Height is 0 || Terrain.Overview is null)
            return;
        Relocate();
        Image = Terrain.Overview.CopyToNewSize(Size);
        var edgeLength = LatticeCell.CellData.EdgeLength;
        var width = args.DrawRect.Width / edgeLength;
        var height = args.DrawRect.Height / edgeLength;
        var x = Terrain.Width - args.Origin.X / (double)edgeLength/* - width / 2*/;
        var y = Terrain.Height - args.Origin.Y / (double)edgeLength/* - height / 2*/;
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
