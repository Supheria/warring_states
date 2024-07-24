using AltitudeMapGenerator.Test;
using AltitudeMapGenerator.VoronoiDiagram.Data;
using LocalUtilities.TypeGeneral;

namespace AltitudeMapGenerator.DLA;

internal class DlaMap(VoronoiCell cell)
{
    Dictionary<Coordinate, DlaPixel> PixelMap { get; } = [];

    VoronoiCell Cell { get; set; } = cell;

    Rectangle Bounds { get; set; } = cell.GetBounds();

    internal double AltitudeMax { get; private set; } = 0;

    public static IProgressor? Progressor { get; set; }
    //#endif
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pixelCount"></param>
    /// <param name="density">[0,1], bigger means that grid-shape is closer to voronoi-cells' shape</param>
    /// <returns></returns>
    internal List<DlaPixel> Generate(int pixelCount, float density)
    {
        PixelMap.Clear();
        AltitudeMax = 0;
        var root = new Coordinate(Cell.Site.X, Cell.Site.Y);
        PixelMap[root] = new(root.X, root.Y);
        bool innerFilter(int x, int y) => Cell.ContainPoint(x, y);
        var count = (int)(pixelCount * density);
        for (int i = 0; PixelMap.Count < count; i++)
        {
            var pixel = AddWalker(innerFilter);
            PixelMap[pixel] = pixel;
        }
        Progressor?.Progress(count);
        bool outerFilter(int x, int y) => Bounds.Contains(x, y);
        for (int i = 0; PixelMap.Count < pixelCount; i++)
        {
            var pixel = AddWalker(outerFilter);
            PixelMap[pixel] = pixel;
        }
        Progressor?.Progress(pixelCount - count);
        ComputeHeight();
        return PixelMap.Values.ToList();
    }

    private DlaPixel AddWalker(Func<int, int, bool> pixelFilter)
    {
        var pixel = new DlaPixel(
                new Random().Next(Bounds.Left, Bounds.Right + 1),
                new Random().Next(Bounds.Top, Bounds.Bottom + 1));
        while (!CheckStuck(pixel))
        {
            int x = pixel.X, y = pixel.Y;
            switch (new Random().Next(0, 8))
            {
                case 0: // left
                    x--;
                    break;
                case 1: // right
                    x++;
                    break;
                case 2: // up
                    y--;
                    break;
                case 3: // down
                    y++;
                    break;
                case 4: // left up
                    x--;
                    y--;
                    break;
                case 5: // up right
                    x++;
                    y--;
                    break;
                case 6: // bottom right
                    x++;
                    y++;
                    break;
                case 7: // left bottom
                    x--;
                    y++;
                    break;
            }
            if (pixelFilter(x, y))
                pixel = new(x, y);
            else
                pixel = new(
                    new Random().Next(Bounds.Left, Bounds.Right + 1),
                    new Random().Next(Bounds.Top, Bounds.Bottom + 1));
        }
        return pixel;
    }

    private bool CheckStuck(DlaPixel pixel)
    {
        var X = pixel.X;
        var Y = pixel.Y;
        var left = X - 1;//Math.Max(x - 1, Bounds.Left);
        var top = Y - 1;//Math.Max(y - 1, Bounds.Top);
        var right = X + 1;//Math.Min(x + 1, Bounds.Right);
        var bottom = Y + 1; //Math.Min(y + 1, Bounds.Bottom);
        bool isStucked = false;
        if (PixelMap.ContainsKey(new(X, Y)))
            return false;
        if (PixelMap.TryGetValue(new(left, Y), out var stucked))
        {
            pixel.Neighbor[Directions.Left] = new(left, Y);
            stucked.Neighbor[Directions.Right] = new(X, Y);
            isStucked = true;
        }
        if (PixelMap.TryGetValue(new(right, Y), out stucked))
        {
            pixel.Neighbor[Directions.Right] = new(right, Y);
            stucked.Neighbor[Directions.Left] = new(X, Y);
            isStucked = true;
        }
        if (PixelMap.TryGetValue(new(X, top), out stucked))
        {
            pixel.Neighbor[Directions.Top] = new(X, top);
            stucked.Neighbor[Directions.Bottom] = new(X, Y);
            isStucked = true;
        }
        if (PixelMap.TryGetValue(new(X, bottom), out stucked))
        {
            pixel.Neighbor[Directions.Bottom] = new(X, bottom);
            stucked.Neighbor[Directions.Top] = new(X, Y);
            isStucked = true;
        }
        if (PixelMap.TryGetValue(new(left, top), out stucked))
        {
            pixel.Neighbor[Directions.LeftTop] = new(left, top);
            stucked.Neighbor[Directions.BottomRight] = new(X, Y);
            isStucked = true;
        }
        if (PixelMap.TryGetValue(new(left, bottom), out stucked))
        {
            pixel.Neighbor[Directions.LeftBottom] = new(left, bottom);
            stucked.Neighbor[Directions.TopRight] = new(X, Y);
            isStucked = true;
        }
        if (PixelMap.TryGetValue(new(right, top), out stucked))
        {
            pixel.Neighbor[Directions.TopRight] = new(right, top);
            stucked.Neighbor[Directions.LeftBottom] = new(X, Y);
            isStucked = true;
        }
        if (PixelMap.TryGetValue(new(right, bottom), out stucked))
        {
            pixel.Neighbor[Directions.BottomRight] = new(right, bottom);
            stucked.Neighbor[Directions.LeftTop] = new(X, Y);
            isStucked = true;
        }
        return isStucked;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pixelMap"></param>
    /// <returns>the max of heights</returns>
    private void ComputeHeight()
    {
        foreach (var pair in PixelMap)
        {
            var pixel = pair.Value;
            CheckDirection(Directions.Left, pixel);
            CheckDirection(Directions.Top, pixel);
            CheckDirection(Directions.Right, pixel);
            CheckDirection(Directions.Bottom, pixel);
            CheckDirection(Directions.LeftTop, pixel);
            CheckDirection(Directions.TopRight, pixel);
            CheckDirection(Directions.LeftBottom, pixel);
            CheckDirection(Directions.BottomRight, pixel);
            AltitudeMax = Math.Max(AltitudeMax, pixel.Altitude);
        }
    }

    private int CheckDirection(Directions direction, DlaPixel walker)
    {
        if (!walker.ConnetNumber.ContainsKey(direction))
        {
            if (walker.Neighbor.TryGetValue(direction, out var neighbor))

                walker.ConnetNumber[direction] = CheckDirection(direction, PixelMap[neighbor]) + 1;
            else
                walker.ConnetNumber[direction] = 0;
        }
        return walker.ConnetNumber[direction];
    }
}
