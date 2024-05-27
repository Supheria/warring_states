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
        if (Terrain.Overview is null)
            Size = new();
        else
        {
            var size = FullScreen ? args.DisplayRect.Size : new((int)(args.DisplayRect.Width * 0.25), (int)(args.DisplayRect.Height * 0.25));
            Size = Terrain.Overview.Size.ScaleSizeOnRatio(size);
            Location = FullScreen ? new((args.DisplayRect.Size - Size) / 2) : new(args.DisplayRect.Right - Width, args.DisplayRect.Top);
        }
    }

    private void Relocate(GridUpdatedArgs args)
    {
        if (Width is 0 || Height is 0 || Terrain.Overview is null)
            return;
        Relocate();
        Image = Terrain.Overview.CopyToNewSize(Size);
        var width = args.DrawRect.Width / LatticeGrid.CellData.EdgeLength;
        var height = args.DrawRect.Height / LatticeGrid.CellData.EdgeLength;
        var x = Terrain.Width - args.Origin.X / /*(double)*/LatticeGrid.CellData.EdgeLength;
        var y = Terrain.Height - args.Origin.Y / /*(double)*/LatticeGrid.CellData.EdgeLength;
        var widthRatio = Terrain.Width / (double)Width;
        var heightRatio = Terrain.Height / (double)Height;
        var rect = new Rectangle((x / widthRatio).ToInt(), (y / heightRatio).ToInt(), (width / widthRatio).ToInt(), (height / heightRatio).ToInt());
        using var g = Graphics.FromImage(Image);
        var rects = rect.CutRectLoopRectsInRange(new(new(0, 0), Size));
        rects.ForEach(r => g.DrawRectangle(Pens.Red, r));
        Invalidate();
    }
}
