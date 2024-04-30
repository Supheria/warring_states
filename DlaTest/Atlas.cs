using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalUtilities.GdiUtilities;
using LocalUtilities.Interface;
using LocalUtilities.VoronoiDiagram;
using LocalUtilities.VoronoiDiagram.Model;

namespace DlaTest;

public class Atlas(int width, int height, int widthSegmentNumber, int heightSegmentNumber, int pixelNumber)
{
    public Dictionary<Coordinate, DlaPixel[]> CellMap { get; } = [];

    public int Width { get; } = width;

    public int Height { get; } = height;

    public Rectangle Bounds => new(0, 0, Width, Height);

    int WidthSegmentNumber { get; } = widthSegmentNumber;

    int HeightSegmentNumber { get; } = heightSegmentNumber;

    public int TotalPixelNumber { get; } = pixelNumber;

    public Atlas() : this(0, 0, 0, 0, 0)
    {

    }

    public void Generate(IPointsGeneration pointGeneration)
    {
        long area = Width * Height;
        foreach (var cell in VoronoiPlane.Generate(Width, Height, WidthSegmentNumber, HeightSegmentNumber, pointGeneration))
            CellMap[cell.Centroid] = DlaMap.Generate(cell, (int)(cell.GetArea() / area * TotalPixelNumber));
    }

#if DEBUG
    [Obsolete("just for test")]
    public List<VoronoiCell> GenerateVoronoi(IPointsGeneration pointGeneration)
    {
        return VoronoiPlane.Generate(Width, Height, WidthSegmentNumber, HeightSegmentNumber, pointGeneration);
    }
#endif
}
