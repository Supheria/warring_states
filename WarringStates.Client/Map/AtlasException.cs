namespace WarringStates.Client.Map;

public class AtlasException(string message) : Exception(message)
{
    public static AtlasException PointOutRange(Coordinate point)
    {
        return new($"{point} is out range of atlas map");
    }
}
