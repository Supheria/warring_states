using AltitudeMapGenerator.Layout;
using LocalUtilities;

namespace AltitudeMapGenerator;

public class AltitudeMapData(Size size, Size segmentNumber, Size riverSegmentNumber, RiverLayout.Types riverLayoutType, double riverWidth, int pixelNumber, float pixelDensity)
{
    public Size Size { get; set; } = size;

    public Size SegmentNumber { get; set; } = segmentNumber;

    public Size RiverSegmentNumber { get; set; } = riverSegmentNumber;

    public double RiverWidth { get; set; } = riverWidth;

    public RiverLayout.Types RiverLayoutType { get; set; } = riverLayoutType;

    public int PixelNumber { get; set; } = pixelNumber;

    public float PixelDensity { get; set; } = pixelDensity;
}
