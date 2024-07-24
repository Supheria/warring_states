using WarringStates.Events;

namespace WarringStates.UI;

public class KeyPressArgs(Keys value) : ICallbackArgs
{
    public Keys Value { get; } = value;
}
