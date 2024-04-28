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

namespace test;

public class DlaMap(VoronoiCell region, int walkerNumber) : Roster<(int, int), DlaWalker>
{
    public int WalkerNumber { get; set; } = walkerNumber;

    public VoronoiCell Region { get; set; } = region;

    public Rectangle Bounds { get; private set; }

    public int HeightMax { get; set; } = 0;

    public DlaMap() : this(new(), 0)
    {

    }

    public bool Generate()
    {
        Bounds = Region.GetBounds();
        //var root = Region.GetCentroid();
        var root = Region.Site;
        RosterMap[root] = new(root);
#if DEBUG
        var testForm = new TestForm()
        {
            Total = WalkerNumber,
        };
        testForm.Show();
#endif
        for (int i = 0; RosterMap.Count < WalkerNumber; i++)
        {
            AddWalker(out var walker);
            RosterMap[(walker.X, walker.Y)] = walker;
            testForm.Now = RosterMap.Count;
            testForm.Progress();
        }
        return true;
    }

    private void AddWalker(out DlaWalker walker)
    {
        walker = new DlaWalker((
                new Random().Next(Bounds.Left, Bounds.Right + 1),
                new Random().Next(Bounds.Top, Bounds.Bottom + 1)
                ));
        while (!walker.CheckStuck(RosterMap))
        {
            int x = walker.X, y = walker.Y;
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
            if (!Region.Contains(x, y))
            {
                x = new Random().Next(Bounds.Left, Bounds.Right + 1);
                y = new Random().Next(Bounds.Top, Bounds.Bottom + 1);
            }
            walker.SetSignature = (x, y);
        } 
    }

    public void ComputeHeight()
    {
        foreach (var pair in RosterMap)
        {
            var walker = pair.Value;
            CheckDirection(Direction.Left, walker);
            CheckDirection(Direction.Top, walker);
            CheckDirection(Direction.Right, walker);
            CheckDirection(Direction.Bottom, walker);
            CheckDirection(Direction.LeftTop, walker);
            CheckDirection(Direction.TopRight, walker);
            CheckDirection(Direction.LeftBottom, walker);
            CheckDirection(Direction.BottomRight, walker);
            var height = walker.Height;
            HeightMax = Math.Max(HeightMax, height);
        }
    }

    private int CheckDirection(Direction direction, DlaWalker walker)
    {
        if (!walker.ConnetNumber.ContainsKey(direction))
        {
            if (walker.Neighbor.TryGetValue(direction, out var neighbor))

                walker.ConnetNumber[direction] = CheckDirection(direction, RosterMap[neighbor]) + 1;
            else
                walker.ConnetNumber[direction] = 0;
        }
        return walker.ConnetNumber[direction];
    }

    public void ResetRelation()
    {
        foreach(var walker in  RosterMap.Values)
        {
            var x = walker.X;
            var y = walker.Y;
            var left = x - 1;//Math.Max(x - 1, Bounds.Left);
            var top = y - 1;//Math.Max(y - 1, Bounds.Top);
            var right = x + 1;//Math.Min(x + 1, Bounds.Right);
            var bottom = y + 1; //Math.Min(y + 1, Bounds.Bottom);
            if (RosterMap.TryGetValue((left, y), out var other) && (!other.Neighbor.ContainsKey(Direction.Right)))
            {
                walker.Neighbor[Direction.Left] = (left, y);
                other.Neighbor[Direction.Right] = (x, y);
            }
            if (RosterMap.TryGetValue((right, y), out other) && (!other.Neighbor.ContainsKey(Direction.Left)))
            {
                walker.Neighbor[Direction.Right] = (right, y);
                other.Neighbor[Direction.Left] = (x, y);
            }
            if (RosterMap.TryGetValue((x, top), out other) && (!other.Neighbor.ContainsKey(Direction.Bottom)))
            {
                walker.Neighbor[Direction.Top] = (x, top);
                other.Neighbor[Direction.Bottom] = (x, y);
            }
            if (RosterMap.TryGetValue((x, bottom), out other) && (!other.Neighbor.ContainsKey(Direction.Top)))
            {
                walker.Neighbor[Direction.Bottom] = (x, bottom);
                other.Neighbor[Direction.Top] = (x, y);
            }
            if (RosterMap.TryGetValue((left, top), out other) && (!other.Neighbor.ContainsKey(Direction.BottomRight)))
            {
                walker.Neighbor[Direction.LeftTop] = (left, top);
                other.Neighbor[Direction.BottomRight] = (x, y);
            }
            if (RosterMap.TryGetValue((left, bottom), out other) && (!other.Neighbor.ContainsKey(Direction.TopRight)))
            {
                walker.Neighbor[Direction.LeftBottom] = (left, bottom);
                other.Neighbor[Direction.TopRight] = (x, y);
            }
            if (RosterMap.TryGetValue((right, top), out other) && (!other.Neighbor.ContainsKey(Direction.LeftBottom)))
            {
                walker.Neighbor[Direction.TopRight] = (right, top);
                other.Neighbor[Direction.LeftBottom] = (x, y);
            }
            if (RosterMap.TryGetValue((right, bottom), out other) && (!other.Neighbor.ContainsKey(Direction.LeftTop)))
            {
                walker.Neighbor[Direction.BottomRight] = (right, bottom);
                other.Neighbor[Direction.LeftTop] = (x, y);
            }
        }
    }
}
