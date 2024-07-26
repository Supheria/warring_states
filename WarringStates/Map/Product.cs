namespace WarringStates.Map;

public class Product
{
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

    long CurrentTickOnTimes { get; set; } = 0;

    public Product(Types type, long amount, int increment)
    {
        Type = type;
        Amount = amount;
        Increment = increment;
        //LocalEvents.Hub.TryAddListener<SpanFlowTickOnArgs>(LocalEvents.Flow.SpanFlowTickOn, IncrementTickOn);
    }

    public Product() : this(Types.None, 0, 0)
    {

    }

    //private void IncrementTickOn(SpanFlowTickOnArgs args)
    //{
    //    if (++CurrentTickOnTimes > IncrementTickOnTimes)
    //    {
    //        Amount += Increment;
    //        CurrentTickOnTimes = 0;
    //    }
    //}
}
