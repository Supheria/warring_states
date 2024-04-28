using LocalUtilities.GdiUtilities;
using LocalUtilities.Interface;
using LocalUtilities.VoronoiDiagram.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace DlaTest;

public class DlaMap()
{
    static Dictionary<(int X, int Y), DlaPixel> PixelMap { get; } = [];

    static VoronoiCell Cell { get; set; } = new();

    static Rectangle Bounds { get; set; }

    public static DlaPixel[] Generate(VoronoiCell cell, int pixelCount)
    {
        Cell = cell;
        Bounds = cell.GetBounds();
        PixelMap.Clear();
        (int X, int Y) root = Cell.Centroid;
        //var root = Region.Site;
        PixelMap[root] = new(root);
#if DEBUG
        var testForm = new TestForm()
        {
            Total = pixelCount,
        };
        testForm.Show();
#endif
        for (int i = 0; PixelMap.Count < pixelCount; i++)
        {
            AddWalker(out var pixel);
            PixelMap[(pixel.X, pixel.Y)] = pixel;
            testForm.Now = PixelMap.Count;
            testForm.Progress();
        }
        ComputeHeight();
        return PixelMap.Values.ToArray();
    }

    private static void AddWalker(out DlaPixel pixel)
    {
        pixel = new DlaPixel((
                new Random().Next(Bounds.Left, Bounds.Right + 1),
                new Random().Next(Bounds.Top, Bounds.Bottom + 1)
                ));
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
            if (!Cell.Contains(x, y))
            {
                x = new Random().Next(Bounds.Left, Bounds.Right + 1);
                y = new Random().Next(Bounds.Top, Bounds.Bottom + 1);
            }
            pixel = new((x, y));
        } 
    }

    private static bool CheckStuck(DlaPixel pixel)
    {
        var X = pixel.X;
        var Y = pixel.Y;
        var left = X - 1;//Math.Max(x - 1, Bounds.Left);
        var top = Y - 1;//Math.Max(y - 1, Bounds.Top);
        var right = X + 1;//Math.Min(x + 1, Bounds.Right);
        var bottom = Y + 1; //Math.Min(y + 1, Bounds.Bottom);
        bool isStucked = false;
        if (PixelMap.ContainsKey((X, Y)))
            return false;
        if (PixelMap.TryGetValue((left, Y), out var stucked))
        {
            pixel.Neighbor[Direction.Left] = (left, Y);
            stucked.Neighbor[Direction.Right] = (X, Y);
            isStucked = true;
        }
        if (PixelMap.TryGetValue((right, Y), out stucked))
        {
            pixel.Neighbor[Direction.Right] = (right, Y);
            stucked.Neighbor[Direction.Left] = (X, Y);
            isStucked = true;
        }
        if (PixelMap.TryGetValue((X, top), out stucked))
        {
            pixel.Neighbor[Direction.Top] = (X, top);
            stucked.Neighbor[Direction.Bottom] = (X, Y);
            isStucked = true;
        }
        if (PixelMap.TryGetValue((X, bottom), out stucked))
        {
            pixel.Neighbor[Direction.Bottom] = (X, bottom);
            stucked.Neighbor[Direction.Top] = (X, Y);
            isStucked = true;
        }
        if (PixelMap.TryGetValue((left, top), out stucked))
        {
            pixel.Neighbor[Direction.LeftTop] = (left, top);
            stucked.Neighbor[Direction.BottomRight] = (X, Y);
            isStucked = true;
        }
        if (PixelMap.TryGetValue((left, bottom), out stucked))
        {
            pixel.Neighbor[Direction.LeftBottom] = (left, bottom);
            stucked.Neighbor[Direction.TopRight] = (X, Y);
            isStucked = true;
        }
        if (PixelMap.TryGetValue((right, top), out stucked))
        {
            pixel.Neighbor[Direction.TopRight] = (right, top);
            stucked.Neighbor[Direction.LeftBottom] = (X, Y);
            isStucked = true;
        }
        if (PixelMap.TryGetValue((right, bottom), out stucked))
        {
            pixel.Neighbor[Direction.BottomRight] = (right, bottom);
            stucked.Neighbor[Direction.LeftTop] = (X, Y);
            isStucked = true;
        }

        return isStucked;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pixelMap"></param>
    /// <returns>the max of heights</returns>
    private static void ComputeHeight()
    {
        foreach (var pair in PixelMap)
        {
            var pixel = pair.Value;
            CheckDirection(Direction.Left, pixel);
            CheckDirection(Direction.Top, pixel);
            CheckDirection(Direction.Right, pixel);
            CheckDirection(Direction.Bottom, pixel);
            CheckDirection(Direction.LeftTop, pixel);
            CheckDirection(Direction.TopRight, pixel);
            CheckDirection(Direction.LeftBottom, pixel);
            CheckDirection(Direction.BottomRight, pixel);
            var height = pixel.Height;
        }
    }

    private static int CheckDirection(Direction direction, DlaPixel walker)
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

    public static Dictionary<(int X, int Y), DlaPixel> RelocatePixels(ICollection<DlaPixel> pixels)
    {
        var pixelMap = new Dictionary<(int X, int Y), DlaPixel>();
        foreach (var pixel in pixels)
            pixelMap[(pixel.X, pixel.Y)] = pixel;
        foreach (var pixel in pixelMap.Values)
        {
            var x = pixel.X;
            var y = pixel.Y;
            var left = x - 1;//Math.Max(x - 1, Bounds.Left);
            var top = y - 1;//Math.Max(y - 1, Bounds.Top);
            var right = x + 1;//Math.Min(x + 1, Bounds.Right);
            var bottom = y + 1; //Math.Min(y + 1, Bounds.Bottom);
            if (pixelMap.TryGetValue((left, y), out var other) && (!other.Neighbor.ContainsKey(Direction.Right)))
            {
                pixel.Neighbor[Direction.Left] = (left, y);
                other.Neighbor[Direction.Right] = (x, y);
            }
            if (pixelMap.TryGetValue((right, y), out other) && (!other.Neighbor.ContainsKey(Direction.Left)))
            {
                pixel.Neighbor[Direction.Right] = (right, y);
                other.Neighbor[Direction.Left] = (x, y);
            }
            if (pixelMap.TryGetValue((x, top), out other) && (!other.Neighbor.ContainsKey(Direction.Bottom)))
            {
                pixel.Neighbor[Direction.Top] = (x, top);
                other.Neighbor[Direction.Bottom] = (x, y);
            }
            if (pixelMap.TryGetValue((x, bottom), out other) && (!other.Neighbor.ContainsKey(Direction.Top)))
            {
                pixel.Neighbor[Direction.Bottom] = (x, bottom);
                other.Neighbor[Direction.Top] = (x, y);
            }
            if (pixelMap.TryGetValue((left, top), out other) && (!other.Neighbor.ContainsKey(Direction.BottomRight)))
            {
                pixel.Neighbor[Direction.LeftTop] = (left, top);
                other.Neighbor[Direction.BottomRight] = (x, y);
            }
            if (pixelMap.TryGetValue((left, bottom), out other) && (!other.Neighbor.ContainsKey(Direction.TopRight)))
            {
                pixel.Neighbor[Direction.LeftBottom] = (left, bottom);
                other.Neighbor[Direction.TopRight] = (x, y);
            }
            if (pixelMap.TryGetValue((right, top), out other) && (!other.Neighbor.ContainsKey(Direction.LeftBottom)))
            {
                pixel.Neighbor[Direction.TopRight] = (right, top);
                other.Neighbor[Direction.LeftBottom] = (x, y);
            }
            if (pixelMap.TryGetValue((right, bottom), out other) && (!other.Neighbor.ContainsKey(Direction.LeftTop)))
            {
                pixel.Neighbor[Direction.BottomRight] = (right, bottom);
                other.Neighbor[Direction.LeftTop] = (x, y);
            }
        }
        return pixelMap;
    }
}
