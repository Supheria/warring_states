using LocalUtilities.TypeGeneral;

namespace AltitudeMapGenerator.Layout;

public static class RiverLayoutType
{
    public static Func<Size, RiverLayout> Parse(this RiverLayout.Types type)
    {
        // [Horizontal]   [Vertical)  [ForwardSlash)  [BackwardSlash)
        //    _______      _______       _______          _______
        //   | _____ |    | |   | |     |    /  |        |  \    |
        //   |       |    | |   | |     |      /|        |\      |
        //   |       |    | |   | |     |/      |        |      \|
        //   | ----- |    | |   | |     |  /    |        |    \  |
        //    -------      -------       -------          -------

        return type switch
        {
            RiverLayout.Types.Horizontal => (size) => new(
                (new(Directions.Left, Operators.LessThan, size), new(Directions.Right, Operators.LessThan, size)),
                (new(Directions.Left, Operators.GreaterThan, size), new(Directions.Right, Operators.GreaterThan, size))
                ),
            RiverLayout.Types.Vertical => (size) => new(
                (new(Directions.Top, Operators.LessThan, size), new(Directions.Bottom, Operators.LessThan, size)),
                (new(Directions.Top, Operators.GreaterThan, size), new(Directions.Bottom, Operators.GreaterThan, size))
                ),
            RiverLayout.Types.ForwardSlash => (size) => new(
                (new(Directions.Top, Operators.GreaterThanOrEqualTo, size), new(Directions.Left, Operators.GreaterThanOrEqualTo, size)),
                (new(Directions.Right, Operators.LessThanOrEqualTo, size), new(Directions.Bottom, Operators.LessThanOrEqualTo, size))
                ),
            RiverLayout.Types.BackwardSlash => (size) => new(
                (new(Directions.Left, Operators.LessThanOrEqualTo, size), new(Directions.Bottom, Operators.GreaterThanOrEqualTo, size)),
                (new(Directions.Top, Operators.LessThanOrEqualTo, size), new(Directions.Right, Operators.GreaterThanOrEqualTo, size))
                ),
            RiverLayout.Types.OneForTest => (size) => new(
                (new(Directions.Top, Operators.GreaterThanOrEqualTo, size), new(Directions.Left, Operators.GreaterThanOrEqualTo, size))
                ),
            _ => throw new InvalidOperationException()
        };
    }
}