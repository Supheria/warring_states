using AtlasGenerator.Test;
using LocalUtilities.TypeGeneral.Convert;
using LocalUtilities.TypeToolKit.EventProcess;
using System.Text;
using WarringStates.UI;

namespace WarringStates.Events;

public static class LocalEvents
{
    public static EventHub TestHub { get; } = new();

    public static EventHub Hub { get; } = new();

    public static class Types
    {
        public enum TestHub
        {
            AddInfo,
            AddInfoList,
        }

        public enum Hub
        {
            GameFormUpdate,
            ImageUpdate,
            GridUpdate
        }
    }

    public static void ForTest()
    {
        var testInfoList = new List<TestForm.TestInfo>();
        foreach (var (type, callback) in Hub.GetEventList())
        {
            if (callback is null)
            {
                testInfoList.Add(new(type.ToWholeString(), "null"));
                continue;
            }
            var callBackNames = new StringBuilder();
            foreach(var c in callback.GetInvocationList())
            {
                var info = $"{c.Target?.ToString()} + {c}";
                testInfoList.Add(new(type.ToWholeString(), info));
                continue;
            }
        }
        TestHub.Broadcast(Types.TestHub.AddInfoList, testInfoList);
    }
}
