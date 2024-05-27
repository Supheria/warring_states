using LocalUtilities.TypeGeneral.Convert;
using LocalUtilities.TypeToolKit.EventProcess;
using System.Text;
using WarringStates.UI;

namespace WarringStates.Events;

public static class LocalEvents
{
    public static EventHub Hub { get; } = new();

    public enum Test
    {
        AddInfo,
        AddInfoList,
        AddSingleInfo,
        AddSingleInfoList,
        AddInfoForMax
    }

    public enum Global
    {
    }

    public enum Graph
    {
        GameImageUpdate,
        GridUpdate,
        PointOnGameImage,
        PointOnCell,
    }

    public enum UserInterface
    {
        GameFormUpdate,
        GameDisplayerUpdate
    }

    public enum Flow
    {
        SpanFlowTickOn,
        SwichFlowState
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
            foreach (var c in callback.GetInvocationList())
            {
                var info = $"{c.Target?.ToString()} + {c}";
                testInfoList.Add(new(type.ToWholeString(), info));
                continue;
            }
        }
        Hub.TryBroadcast(Test.AddInfoList, testInfoList);
    }
}
