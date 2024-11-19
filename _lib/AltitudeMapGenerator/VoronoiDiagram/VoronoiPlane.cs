﻿using AltitudeMapGenerator.VoronoiDiagram.BorderDisposal;
using AltitudeMapGenerator.VoronoiDiagram.Data;
using AltitudeMapGenerator.VoronoiDiagram.Model;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Graph;
using LocalUtilities.TypeToolKit.Mathematic;

namespace AltitudeMapGenerator.VoronoiDiagram;

/// <summary>
/// An Euclidean plane where a Voronoi diagram can be constructed from <see cref="VoronoiCell"/>s
/// producing a tesselation of cells with <see cref="VoronoiEdge"/> line segments and <see cref="VoronoiVertex"/> vertices.
/// </summary>
internal class VoronoiPlane(Size size)
{
    List<VoronoiCell> Cells { get; set; } = [];

    List<VoronoiEdge> Edges { get; set; } = [];

    int Width { get; } = size.Width;

    int Height { get; } = size.Height;

    internal List<Coordinate> GenerateSites(Size segmentNumber)
    {
        return GenerateSites(segmentNumber, []);
    }

    internal List<Coordinate> GenerateSites(Size segmentNumber, List<Coordinate> existedSites)
    {
        var widthSegment = Width / segmentNumber.Width;
        var heightSegment = Height / segmentNumber.Height;
        var excludes = new Dictionary<(int, int), Coordinate>();
        foreach (var site in existedSites)
        {
            var key = (site.X / widthSegment, site.Y / heightSegment);
            excludes[key] = site;
        }
        var sites = new List<Coordinate>();
        var random = new Random();
        for (int i = 0; i < segmentNumber.Width; i++)
        {
            for (int j = 0; j < segmentNumber.Height; j++)
            {
                if (excludes.ContainsKey((i, j)))
                    continue;
                var (X, Y) = PointGenerator.GeneratePoint(random, widthSegment * i, heightSegment * j, widthSegment * (i + 1), heightSegment * (j + 1), 1).First();
                sites.Add(new(X.ToRoundInt(), Y.ToRoundInt()));
            }
        }
        sites.AddRange(existedSites);
        return sites;
    }



    /// <summary>
    /// The generated sites are guaranteed not to lie on the border of the plane (although they may be very close).
    /// Multi times to use will stack on points last generated 
    /// </summary>
    internal List<VoronoiCell> Generate(List<Coordinate> sites)
    {
        Cells = UniquePoints(sites).Select(c => new VoronoiCell(c)).ToList();
        Edges.Clear();
        Generate();
        return Cells;
    }

    private static List<Coordinate> UniquePoints(List<Coordinate> coordinates)
    {
        coordinates.Sort((p1, p2) =>
        {
            if (p1.X == p2.X)
            {
                if (p1.Y == p2.Y)
                    return 0;
                if (p1.Y < p2.Y)
                    return -1;
                return 1;
            }
            if (p1.X < p2.X)
                return -1;
            return 1;
        });
        var unique = new List<Coordinate>();
        var last = coordinates.First();
        unique.Add(last);
        for (var index = 1; index < coordinates.Count; index++)
        {
            var coordiante = coordinates[index];
            if (last.X != coordiante.X ||
                last.Y != coordiante.Y)
            {
                unique.Add(coordiante);
                last = coordiante;
            }
        }
        return unique;
    }

    private void Generate()
    {
        var eventQueue = new MinHeap<IFortuneEvent>(5 * Cells.Count);
        foreach (var site in Cells)
            eventQueue.Insert(new FortuneSiteEvent(site));
        //init tree
        var beachLine = new BeachLine();
        var edges = new LinkedList<VoronoiEdge>();
        var deleted = new HashSet<FortuneCircleEvent>();
        //init edge list
        while (eventQueue.Count != 0)
        {
            IFortuneEvent fEvent = eventQueue.Pop();
            if (fEvent is FortuneSiteEvent fsEvent)
                beachLine.AddBeachSection(fsEvent, eventQueue, deleted, edges);
            else
            {
                if (deleted.Contains((FortuneCircleEvent)fEvent))
                    deleted.Remove((FortuneCircleEvent)fEvent);
                else
                    beachLine.RemoveBeachSection((FortuneCircleEvent)fEvent, eventQueue, deleted, edges);
            }
        }
        Edges = edges.ToList();
        Edges = BorderClipping.Clip(Edges, 0, 0, Width, Height);
        Edges = BorderClosing.Close(Edges, 0, 0, Width, Height, Cells);
    }
}
