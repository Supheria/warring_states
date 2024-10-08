﻿using LocalUtilities.TypeToolKit.Convert;
using System.Collections.Concurrent;
using System.Text;
using WarringStates.Client.UI;
using WarringStates.Events;

namespace WarringStates.Client.Events;

public class LocalEvents
{
    static ConcurrentDictionary<Enum, Delegate> EventMap { get; } = [];

    public static bool TryAddListener(Enum eventType, Callback callback)
    {
        try
        {
            var success = EventMap.TryAdd(eventType, callback);
            if (!EventMap.TryGetValue(eventType, out var exist))
                return false;
            if (success)
                return true;
            EventMap[eventType] = (Callback)exist + callback;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool TryAddListener<TArgs>(Enum eventType, Callback<TArgs> callback) where TArgs : EventArgs
    {
        try
        {
            var success = EventMap.TryAdd(eventType, callback);
            if (!EventMap.TryGetValue(eventType, out var exist))
                return false;
            if (success)
                return true;
            EventMap[eventType] = (Callback<TArgs>)exist + callback;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool TryRemoveListener(Enum eventType, Callback callback)
    {
        try
        {
            if (!EventMap.TryGetValue(eventType, out var exist))
                return false;
            exist = (Callback)exist - callback;
            if (exist is null)
                return EventMap.TryRemove(eventType, out _);
            EventMap[eventType] = exist;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool TryRemoveListener<TArgs>(Enum eventType, Callback<TArgs> callback) where TArgs : EventArgs
    {
        try
        {
            if (!EventMap.TryGetValue(eventType, out var exist))
                return false;
            exist = (Callback<TArgs>)exist - callback;
            if (exist is null)
                return EventMap.TryRemove(eventType, out _);
            EventMap[eventType] = exist;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool TryBroadcast(Enum eventType)
    {
        if (!EventMap.TryGetValue(eventType, out var callback))
            return false;
        callback.DynamicInvoke();
        return true;
    }

    public static bool TryBroadcast<TArgs>(Enum eventType, TArgs args) where TArgs : EventArgs
    {
        try
        {
            if (!EventMap.TryGetValue(eventType, out var callback))
                return false;
            callback.DynamicInvoke(args);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public enum Test
    {
        AddInfo,
        AddInfoList,
        AddSingleInfo,
        AddSingleInfoList,
        ValueForMax,
        AddValue,
        ClearValue,
    }

    public enum Map
    {
        AtlasUpdate,
    }

    public enum Graph
    {
        OperateGridOrigin,

        GridToRelocate,
        GridRedraw,
        PointOnGridCell,
        PointOnCell,
        GridOriginToOffset,
        GridOriginToReset,
        GridReset,

    }

    public enum UserInterface
    {
        ArchiveListRefreshed,
        CurrentArchiveChange,
        SourceLandCanBuild,

        GamePlayControlOnDraw,
        SettingsOnSetBounds,
        ToolBarOnSetBounds,
        InfoBarOnSetBounds,
        ArchiveSelected,
        StartGamePlay,
        FinishGamePlay,
        KeyPressed,
        EndGame,
    }

    public enum Flow
    {
        SpanFlowTickOn,
        SwichFlowState,
        AnimateFlowTickOn
    }

    public static void ForTest()
    {
        var testinfolist = new List<TestForm.StringInfo>();
        foreach (var (type, callback) in EventMap)
        {
            if (callback is null)
            {
                testinfolist.Add(new(type.ToWholeString(), "null"));
                continue;
            }
            var callbacknames = new StringBuilder();
            var i = 0;
            foreach (var c in callback.GetInvocationList())
            {
                var info = $"{c.Target?.ToString()} + {c}";
                testinfolist.Add(new($"{type.ToWholeString()}: {i} => {c.Target}", ""));
                i++;
            }
        }
        //TryBroadcast(Test.AddInfoList, testinfolist);
    }
}
