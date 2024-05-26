namespace WarringStates;

internal class LoopException(string message) : Exception(message)
{
    public static LoopException ValueOutRange<T>(int value)
    {
        return new($"{value} is out range of {typeof(T).Name}");
    }
}
