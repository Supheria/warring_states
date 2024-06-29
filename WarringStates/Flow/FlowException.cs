namespace WarringStates;

internal class FlowException(string message) : Exception(message)
{
    public static FlowException ValueOutRange<T>(int value)
    {
        return new($"{value} is out range of {typeof(T).Name}");
    }
}
