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
                (new(Directions.Left, OperatorTypes.LessThan, size), new(Directions.Right, OperatorTypes.LessThan, size)),
                (new(Directions.Left, OperatorTypes.GreaterThan, size), new(Directions.Right, OperatorTypes.GreaterThan, size))
                ),
            RiverLayout.Types.Vertical => (size) => new(
                (new(Directions.Top, OperatorTypes.LessThan, size), new(Directions.Bottom, OperatorTypes.LessThan, size)),
                (new(Directions.Top, OperatorTypes.GreaterThan, size), new(Directions.Bottom, OperatorTypes.GreaterThan, size))
                ),
            RiverLayout.Types.ForwardSlash => (size) => new(
                (new(Directions.Top, OperatorTypes.GreaterThanOrEqualTo, size), new(Directions.Left, OperatorTypes.GreaterThanOrEqualTo, size)),
                (new(Directions.Right, OperatorTypes.LessThanOrEqualTo, size), new(Directions.Bottom, OperatorTypes.LessThanOrEqualTo, size))
                ),
            RiverLayout.Types.BackwardSlash => (size) => new(
                (new(Directions.Left, OperatorTypes.LessThanOrEqualTo, size), new(Directions.Bottom, OperatorTypes.GreaterThanOrEqualTo, size)),
                (new(Directions.Top, OperatorTypes.LessThanOrEqualTo, size), new(Directions.Right, OperatorTypes.GreaterThanOrEqualTo, size))
                ),
            RiverLayout.Types.OneForTest => (size) => new(
                (new(Directions.Top, OperatorTypes.GreaterThanOrEqualTo, size), new(Directions.Left, OperatorTypes.GreaterThanOrEqualTo, size))
                ),
            _ => throw new InvalidOperationException()
        };
    }
}