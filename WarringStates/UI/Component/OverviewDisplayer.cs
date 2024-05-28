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
        LocalEvents.Hub.AddListener<GameDisplayerUpdateArgs>(LocalEvents.UserInterface.GameDisplayerUpdate, SetBounds);
        LocalEvents.Hub.AddListener<GridUpdatedArgs>(LocalEvents.Graph.GridUpdate, Relocate);
    }

    private void SetBounds(GameDisplayerUpdateArgs args)
    {
        if (Atlas.Overview is null)
            Size = new();
        else
        {
            var size = FullScreen ? args.DisplayRect.Size : new((int)(args.DisplayRect.Width * 0.25), (int)(args.DisplayRect.Height * 0.25));
            Size = Atlas.Overview.Size.ScaleSizeOnRatio(size);
            Location = FullScreen ? new((args.DisplayRect.Size - Size) / 2) : new(args.DisplayRect.Right - Width, args.DisplayRect.Top);
        }
    }

    private void Relocate(GridUpdatedArgs args)
    {
        if (Width is 0 || Height is 0 || Atlas.Overview is null)
            return;
        Relocate();
        Image = Atlas.Overview.CopyToNewSize(Size);
        var width = args.DrawRect.Width / LatticeGrid.CellEdgeLength;
        var height = args.DrawRect.Height / LatticeGrid.CellEdgeLength;
        var x = Atlas.Width - args.Origin.X / /*(double)*/LatticeGrid.CellEdgeLength;
        var y = Atlas.Height - args.Origin.Y / /*(double)*/LatticeGrid.CellEdgeLength;
        var widthRatio = Atlas.Width / (double)Width;
        var heightRatio = Atlas.Height / (double)Height;
        var rect = new Rectangle((x / widthRatio).ToInt(), (y / heightRatio).ToInt(), (width / widthRatio).ToInt(), (height / heightRatio).ToInt());
        using var g = Graphics.FromImage(Image);
        var rects = rect.CutRectLoopRectsInRange(new(new(0, 0), Size));
        rects.ForEach(r => g.DrawRectangle(Pens.Red, r));
        Invalidate();
    }
}
