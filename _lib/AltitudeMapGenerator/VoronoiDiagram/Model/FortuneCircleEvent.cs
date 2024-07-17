﻿using AltitudeMapGenerator.VoronoiDiagram.Data;

namespace AltitudeMapGenerator.VoronoiDiagram.Model;

internal class FortuneCircleEvent : IFortuneEvent
{
    internal VoronoiVertex Lowest { get; }
    internal double YCenter { get; }
    internal RBTreeNode<BeachSection> ToDelete { get; }

    internal FortuneCircleEvent(VoronoiVertex lowest, double yCenter, RBTreeNode<BeachSection> toDelete)
    {
        Lowest = lowest;
        YCenter = yCenter;
        ToDelete = toDelete;
    }

    public int CompareTo(IFortuneEvent? other)
    {
        if (other is null)
        {
            if (this is null)
                return 0;
            else
                throw VoronoiException.NullFortuneCircleEvent(nameof(other));
        }
        int c = Y.CompareTo(other.Y);
        return c == 0 ? X.CompareTo(other.X) : c;
    }

    public double X => Lowest.X;
    public double Y => Lowest.Y;
}
