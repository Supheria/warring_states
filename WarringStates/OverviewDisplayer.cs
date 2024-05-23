using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Graph;
using LocalUtilities.TypeToolKit.Mathematic;

namespace WarringStates;

public class OverviewDisplayer : Displayer
{
    bool FullScreen { get; set; } = false;

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
        Image = Terrain.Overview?.CopyToNewSize(Size);
        Image?.Save("_OverviewDisplayer.bmp");
    }
}
