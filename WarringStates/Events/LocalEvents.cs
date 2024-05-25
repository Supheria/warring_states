using LocalUtilities.TypeGeneral.Convert;
using LocalUtilities.TypeToolKit.EventProcess;
using System.Text;
using WarringStates.UI;

namespace WarringStates.Events;

public static class LocalEvents
{
    public static EventHub Test { get; } = new();

    public static EventHub Global { get; } = new();

    public static EventHub Loop { get; } = new();

    public static class Types
    {
        public enum Test
        {
            AddInfo,
            AddInfoList,
        }

        public enum Global
        {
            TimeTickOn,
            GameFormUpdate,
            ImageUpdate,
            GridUpdate
        }

        public enum Loop
        {
            StopSpanFlow,
            StartSpanFlow,
        }
    }

    public static void ForTest()
    {
        var testInfoList = new List<TestForm.TestInfo>();
        foreach (var (type, callback) in Global.GetEventList())
        {
            if (callback is null)
            {
                testInfoList.Add(new(type.ToWholeString(), "null"));
                continue;
            }
            var callBackNames = new StringBuilder();
            foreach (var c in callback.GetInvocationList())
            {
                var info = $"{c.Target?.ToString()} + {c}";
                testInfoList.Add(new(type.ToWholeString(), info));
                continue;
            }
        }
        Test.Broadcast(Types.Test.AddInfoList, testInfoList);
    }
}
