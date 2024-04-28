using LocalUtilities.GdiUtilities;
using LocalUtilities.Interface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DlaTest;

public enum Direction
{
    None,
    Left,
    Top,
    Right,
    Bottom,
    LeftTop,
    TopRight,
    LeftBottom,
    BottomRight,
}

public class DlaPixel((int X, int Y) point)
{
    public int X => point.X;

    public int Y => point.Y;

    public Dictionary<Direction, (int X, int Y)> Neighbor { get; } = new();

    public Dictionary<Direction, int> ConnetNumber { get; } = new();

    public int Height
    {
        get
        {
            if (_height is -1)
            {
                if (!ConnetNumber.TryGetValue(Direction.Top, out var top))
                    top = 0;
                if (!ConnetNumber.TryGetValue(Direction.Bottom, out var bottom))
                    bottom = 0;
                if (!ConnetNumber.TryGetValue(Direction.Left, out var left))
                    left = 0;
                if (!ConnetNumber.TryGetValue(Direction.Right, out var right))
                    right = 0;
                if (!ConnetNumber.TryGetValue(Direction.LeftTop, out var leftTop))
                    leftTop = 0;
                if (!ConnetNumber.TryGetValue(Direction.BottomRight, out var bottomRight))
                    bottomRight = 0;
                if (!ConnetNumber.TryGetValue(Direction.TopRight, out var topRight))
                    topRight = 0;
                if (!ConnetNumber.TryGetValue(Direction.LeftBottom, out var leftBottom))
                    leftBottom = 0;
                _height = Math.Max(
                    Math.Max(
                        top + bottom - Math.Abs(top - bottom),
                        left + right - Math.Abs(left - right)),
                    Math.Max(
                        leftTop + bottomRight - Math.Abs(leftTop - bottomRight),
                        topRight + leftBottom - Math.Abs(topRight - leftBottom))
                    );
            }
            return _height;
        }
        set => _height = value;
    }
    int _height = -1;

    public DlaPixel() : this((0, 0))
    {

    }
}

