

using LocalUtilities.TypeToolKit.EventProcess;

namespace WarringStates;

public class EventId
{
    public const int Open = 100;
    public const int Close = 101;
    public const int Message = 102;
}

public class TestMsg
{
    public int MsgId { get; set; }

    public string Msg { get; set; } = "";
}

public class ClassA
{
    public void DoTest()
    {
        var arg1 = new EventArgument<string>("打开");
        EventManager.Instance.Dispatch(EventId.Open, arg1);

        var arg2 = new EventArgument<int>(999);
        EventManager.Instance.Dispatch(EventId.Close, arg2);

        var value = new TestMsg()
        {
            MsgId = 888,
            Msg = "一条广告"
        };
        IEventArgument arg3 = new EventArgument<TestMsg>(value);
        EventManager.Instance.Dispatch(EventId.Message, arg3);
    }
}

public class ClassB : IEventListener
{
    public void OnEnable()
    {
        EventManager.Instance.AddEvent(EventId.Open, this);
        EventManager.Instance.AddEvent(EventId.Close, this);
        EventManager.Instance.AddEvent(EventId.Message, this);
    }

    public void OnDisable()
    {
        EventManager.Instance.RemoveEvent(EventId.Open, this);
        EventManager.Instance.RemoveEvent(EventId.Close, this);
        EventManager.Instance.RemoveEvent(EventId.Message, this);
    }

    public void HandleEvent(int eventId, IEventArgument argument)
    {
        switch (eventId)
        {
            case EventId.Open:
                string value1 = argument.GetValue<string>() ?? "";
                MessageBox.Show(value1);
                break;

            case EventId.Close:
                int value2 = argument.GetValue<int>();
                MessageBox.Show(value2.ToString());
                break;

            case EventId.Message:
                TestMsg value3 = argument.GetValue<TestMsg>() ?? new();
                MessageBox.Show(value3.Msg);
                break;
            default:
                break;
        }
    }
}
