using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral.Convert;
using WarringStates.Events;

namespace WarringStates.Map.Terrain;

public class Product : ISsSerializable
{
    public string LocalName => nameof(Product);

    public enum Types
    {
        None,
        FoodStuff,
        Money
    }

    public Types Type { get; private set; }

    public long Amount { get; private set; }

    public int Increment { get; private set; }

    public static int IncrementTickOnTimes { get; } = 30;

    int CurrentTickOnTimes { get; set; } = 0;

    public Product(Types type, long amount, int increment)
    {
        Type = type;
        Amount = amount;
        Increment = increment;
        LocalEvents.Hub.TryAddListener<SpanFlowTickOnArgs>(LocalEvents.Flow.SpanFlowTickOn, IncrementTickOn);
    }

    public Product() : this(Types.None, 0, 0)
    {

    }

    private void IncrementTickOn(SpanFlowTickOnArgs args)
    {
        if (++CurrentTickOnTimes > IncrementTickOnTimes)
        {
            Amount += Increment;
            CurrentTickOnTimes = 0;
        }
    }

    public void Serialize(SsSerializer serializer)
    {
        serializer.WriteTag(nameof(Type), Type.ToString());
        serializer.WriteTag(nameof(Amount), Amount.ToString());
        serializer.WriteTag(nameof(Increment), Increment.ToString());
        serializer.WriteTag(nameof(CurrentTickOnTimes), CurrentTickOnTimes.ToString());
    }

    public void Deserialize(SsDeserializer deserializer)
    {
        Type = deserializer.ReadTag(nameof(Type), s => s.ToEnum<Types>());
        Amount = deserializer.ReadTag(nameof(Amount), long.Parse);
        Increment = deserializer.ReadTag(nameof(Increment), int.Parse);
        CurrentTickOnTimes = deserializer.ReadTag(nameof(CurrentTickOnTimes), int.Parse);
    }
}
