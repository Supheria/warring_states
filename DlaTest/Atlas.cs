using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalUtilities.GdiUtilities;
using LocalUtilities.Interface;
using LocalUtilities.VoronoiDiagram;

namespace DlaTest;

public class Atlas(int width, int height, int widthSegmentNumber, int heightSegmentNumber, int pixelNumber)
{
    public Dictionary<(int X, int Y), DlaPixel[]> BlockMap { get; } = [];

    public int Width { get; } = width;

    public int Height { get; } = height;

    public Rectangle Bounds => new(0, 0, Width, Height);

    int WidthSegmentNumber { get; } = widthSegmentNumber;

    int HeightSegmentNumber { get; } = heightSegmentNumber;

    public int TotalPixelNumber { get; } = pixelNumber;

    public Atlas() : this(0, 0, 0, 0, 0)
    {

    }

    public void Generate()
    {
        var sites = new List<(double, double)>();
        var pointsGenerator = new RandomPointsGenerationGaussian();
        var widthSegment = Width / WidthSegmentNumber;
        var heightSegment = Height / HeightSegmentNumber;
        for (int i = 0; i < WidthSegmentNumber; i++)
        {
            for (int j = 0; j < HeightSegmentNumber; j++)
                sites.Add(
                    pointsGenerator.Generate(widthSegment * i, heightSegment * j, widthSegment * (i + 1), heightSegment * (j + 1), 1).First()
                    );
        }
        var voronoiPlane = new VoronoiPlane(0, 0, Width, Height);
        voronoiPlane.Generate(sites);
        long area = Width * Height;
        foreach (var cell in voronoiPlane.Cells)
            BlockMap[cell.Centroid] = DlaMap.Generate(cell, (int)(cell.GetArea() / area * TotalPixelNumber));
    }
}
