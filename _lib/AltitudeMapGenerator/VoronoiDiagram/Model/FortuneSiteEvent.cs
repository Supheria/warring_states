﻿using AltitudeMapGenerator.VoronoiDiagram.Data;

namespace AltitudeMapGenerator.VoronoiDiagram.Model;

internal class FortuneSiteEvent : IFortuneEvent
{
    public double X => Cell.Site.X;
    public double Y => Cell.Site.Y;
    internal VoronoiCell Cell { get; }

    internal FortuneSiteEvent(VoronoiCell cell)
    {
        Cell = cell;
    }

    public int CompareTo(IFortuneEvent? other)
    {
        if (other is null)
        {
            if (this is null)
                return 0;
            else
                throw VoronoiException.NullFortuneSiteEvent(nameof(other));
        }
        int c = Y.CompareTo(other.Y);
        return c == 0 ? X.CompareTo(other.X) : c;
    }

}